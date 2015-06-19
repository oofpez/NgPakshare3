using PostSharp.Aspects;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Wimt.CachingFramework.Logging;

namespace Wimt.CachingFramework
{
    [Serializable]
    public sealed class CacheAttribute : MethodInterceptionAspect
    {
        /// <summary>
        /// Days the element should live in the cache.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Hours the element should live in the cache.
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// Minutes the element should live in the cache.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Seconds the element should live in the cache.
        /// </summary>
        public int Seconds { get; set; }

        private TimeSpan Lifespan
        {
            get
            {
                return new TimeSpan(Days, Hours, Minutes, Seconds);
            }
        }

        private BaseCache Cache
        {
            get
            {
                return CacheManager.Instance.GetCache(cacheName);
            }
        }

        private string cacheName;

        public bool LogCacheHits { get; private set; }

        public bool LogCacheMisses { get; private set; }

        public bool LogCacheIgnores { get; private set; }

        public bool LogCacheRaceConditions { get; private set; }

        public bool LogCacheNulls { get; private set; }

        [NonSerialized]
        private object syncRoot;

        private CacheKeyBuilder keyBuilder;

        /// <summary>
        /// Caches the output of a method based on the parameters' ToString values.
        /// </summary>
        /// <param name="name">The name of the cache</param>
        public CacheAttribute(
            string name,
            bool logCacheHits = false,
            bool logCacheMisses = false,
            bool logCacheIgnores = false,
            bool logCacheRaceConditions = false,
            bool logCacheNulls = false)
        {
            cacheName = name;
            LogCacheHits = logCacheHits;
            LogCacheMisses = logCacheMisses;
            LogCacheIgnores = logCacheIgnores;
            LogCacheRaceConditions = logCacheRaceConditions;
            LogCacheNulls = logCacheNulls;
        }

        public override void RuntimeInitialize(MethodBase method)
        {
            syncRoot = new object();
        }

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var executionTimer = Stopwatch.StartNew();

            var cacheKey = keyBuilder.GetFriendlyKey(args);

            CacheEvent cacheEvent;

            if (Cache.IsEnabled && CacheManager.Instance.IsCachingEnabled)
            {
                var cachedValue = Cache.Get(cacheKey);

                if (cachedValue != null)
                {
                    args.ReturnValue = cachedValue;
                    cacheEvent = CacheEvent.CacheHit;
                }
                else
                {
                    var returnValue = args.Invoke(args.Arguments);
                    args.ReturnValue = returnValue;

                    if (returnValue != null)
                    {
                        if (Lifespan != null && Lifespan > TimeSpan.Zero)
                        {
                            Cache.Set(cacheKey, returnValue, Lifespan);
                        }
                        else
                        {
                            Cache.Set(cacheKey, returnValue);
                        }

                        cacheEvent = CacheEvent.CacheMiss;
                    }
                    else
                    {
                        cacheEvent = CacheEvent.CacheNull;
                    }
                }
            }
            else
            {
                args.ReturnValue = args.Invoke(args.Arguments);
                cacheEvent = CacheEvent.CacheIgnored;
            }

            if (CacheManager.Instance.IsEventLoggingEnabled)
            {
                if ((cacheEvent == CacheEvent.CacheHit && LogCacheHits) ||
                    (cacheEvent == CacheEvent.CacheMiss && LogCacheMisses) ||
                    (cacheEvent == CacheEvent.CacheIgnored && LogCacheIgnores) ||
                    (cacheEvent == CacheEvent.CacheRaceCondition && LogCacheRaceConditions) ||
                    (cacheEvent == CacheEvent.CacheNull && LogCacheNulls))
                {
                    CacheManager.Instance.Logger.LogCacheEvent(Cache.Name, cacheEvent, Convert.ToInt32(executionTimer.ElapsedMilliseconds), cacheKey);
                }
            }
        }

        public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
        {
            var key = new StringBuilder();
            key.Append(method.DeclaringType.Namespace).Append(".");
            key.Append(method.DeclaringType.Name).Append(".");
            key.Append(method.Name);

            if (method.IsGenericMethod)
            {
                key.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    key.Append(g.ToString());
                    key.Append(",");
                }
                key.Remove(key.Length - 1, 1).Append(">");
            }

            keyBuilder = new CacheKeyBuilder(key.ToString());
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            if (method.GetParameters().Any(p => p.GetType().Equals(typeof(String)) && typeof(IEnumerable).IsAssignableFrom(p.ParameterType)))
            {
                throw new ArgumentOutOfRangeException("Cannot create a key from type IEnumerable");
            }

            return base.CompileTimeValidate(method);
        }
    }
}