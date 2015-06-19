using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wimt.CachingFramework.Caches;
using Wimt.CachingFramework.Configuration;
using Wimt.CachingFramework.Logging;

namespace Wimt.CachingFramework
{
    public class CacheManager : IDisposable
    {
        protected Dictionary<string, BaseCache> CacheDictionary { get; private set; }

        private static CacheManager instance;

        public static CacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CacheManager();
                }

                return instance;
            }
        }

        public bool IsCachingEnabled
        {
            get
            {
                return Settings.IsCachingEnabled();
            }
        }

        public bool IsEventLoggingEnabled
        {
            get
            {
                return Settings.IsEventLoggingEnabled();
            }
        }

        public ICacheLogger Logger { get; private set; }

        public IConfigurationSettings Settings { get; private set; }

        private CacheManager()
        {
            CacheDictionary = new Dictionary<string, BaseCache>();
        }

        public BaseCache GetCache(string name)
        {
            return CacheDictionary[name];
        }

        public bool HasCache(string name)
        {
            return CacheDictionary.Keys.Contains(name);
        }

        public IEnumerable<BaseCache> GetCaches()
        {
            return CacheDictionary.Values.ToList();
        }

        public void Initialise(ICacheLogger logger, IConfigurationSettings settings)
        {
            Logger = logger;
            Settings = settings;

            Logger.LogInfo("CachingFramework initialising.");

            var configSection = ConfigurationManager.GetSection("cachingFramework") as CacheConfigSection;

            if (configSection == null)
            {
                throw new ConfigurationErrorsException("No 'cachingFramework' section found in the configuration file.");
            }

            foreach (var inMemoryCacheSection in configSection.InMemoryCaches.Cast<InMemoryCacheElement>())
            {
                if (CacheDictionary.Keys.Contains(inMemoryCacheSection.Name))
                {
                    throw new ConfigurationErrorsException(String.Format("'{0}' already exists", inMemoryCacheSection.Name));
                }

                var cache = InMemoryCache.InitialiseFrom(inMemoryCacheSection);

                CacheDictionary.Add(cache.Name, cache);

                Logger.LogInfo(cache.Name, "InMemoryCache created");
            }

            foreach (var inRoleCacheSection in configSection.InRoleCaches.Cast<InRoleCacheElement>())
            {
                if (CacheDictionary.Keys.Contains(inRoleCacheSection.Name))
                {
                    throw new ConfigurationErrorsException(String.Format("'{0}' already exists", inRoleCacheSection.Name));
                }

                var cache = InRoleCache.InitialiseFrom(inRoleCacheSection, Logger);

                CacheDictionary.Add(cache.Name, cache);

                Logger.LogInfo(cache.Name, "InRoleCache created");
            }

            foreach (var blobCacheSection in configSection.BlobCaches.Cast<BlobCacheElement>())
            {
                if (CacheDictionary.Keys.Contains(blobCacheSection.Name))
                {
                    throw new ConfigurationErrorsException(String.Format("'{0}' already exists", blobCacheSection.Name));
                }

                var cache = BlobCache.InitialiseFrom(blobCacheSection);

                CacheDictionary.Add(cache.Name, cache);

                Logger.LogInfo(cache.Name, "BlobCache created");
            }

            Logger.LogInfo("CachingFramework initialised.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Logger != null)
                {
                    Logger.LogInfo("CachingFramework disposing.");
                }

                if (CacheDictionary != null)
                {
                    foreach (var cache in CacheDictionary.Values)
                    {
                        cache.Dispose();
                    }
                }
            }
        }
    }
}