using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using Wimt.CachingFramework.Configuration;

namespace Wimt.CachingFramework.Caches
{
    public class InMemoryCache : BaseCache
    {
        private MemoryCache cache;

        public InMemoryCache(string name, bool isEnabled, NameValueCollection config)
            : base(name, isEnabled)
        {
            cache = new MemoryCache(name, config);
        }

        public override object Get(string key)
        {
            return cache.Get(key);
        }

        public override void Set(string key, object value)
        {
            cache.Set(key, value, new CacheItemPolicy());
        }

        public override void Remove(string key)
        {
            cache.Remove(key);
        }

        public override void Clear()
        {
            foreach (string cacheKey in cache.Select(kvp => kvp.Key).ToList())
            {
                cache.Remove(cacheKey);
            }
        }

        public override void Set(string key, object value, DateTime expiresAt)
        {
            var policy = new CacheItemPolicy();

            if (expiresAt > DateTime.UtcNow)
            {
                policy.AbsoluteExpiration = expiresAt;
            }

            cache.Set(key, value, policy);
        }

        public override void Set(string key, object value, TimeSpan validFor)
        {
            var policy = new CacheItemPolicy();

            if (validFor.Ticks > 0)
            {
                policy.AbsoluteExpiration = DateTime.UtcNow.Add(validFor);
            }

            cache.Set(key, value, policy);
        }

        public override bool Exists(string key)
        {
            return cache.Any(x => x.Key == key);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cache != null)
                {
                    cache.Dispose();
                    cache = null;
                }
            }
        }

        internal static InMemoryCache InitialiseFrom(InMemoryCacheElement element)
        {
            var configuration = new NameValueCollection();
            configuration.Add("CacheMemoryLimitMegabytes", element.CacheMemoryLimitMegabytes.ToString());
            configuration.Add("PhysicalMemoryLimitPercentage", element.PhysicalMemoryLimitPercentage.ToString());
            configuration.Add("PollingInterval", element.MemoryCheckPollingInterval.ToString());

            return new InMemoryCache(element.Name, element.IsEnabled, configuration);
        }
    }
}