using Microsoft.ApplicationServer.Caching;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using Wimt.CachingFramework.Configuration;
using Wimt.CachingFramework.Logging;

namespace Wimt.CachingFramework.Caches
{
    public class InRoleCache : BaseCache
    {
        private DataCache DataCache
        {
            get
            {
                if (dataCacheFactory == null)
                {
                    Initialise();
                }

                return dataCacheFactory.GetCache(Name);
            }
        }

        private ICacheLogger cacheLogger;

        private DataCacheFactory dataCacheFactory;

        private DataCacheFactoryConfiguration dataCacheFactoryConfiguration;

        private RetryPolicy<CacheTransientErrorDetectionStrategy> retryPolicy;

        public InRoleCache(string name, bool isEnabled, DataCacheFactoryConfiguration configuration, ICacheLogger logger)
            : base(name, isEnabled)
        {
            dataCacheFactoryConfiguration = configuration;
            cacheLogger = logger;
            retryPolicy = new RetryPolicy<CacheTransientErrorDetectionStrategy>(RetryStrategy.DefaultClientRetryCount);

            if (isEnabled)
            {
                Initialise();
            }
        }

        private void Initialise()
        {
            dataCacheFactory = new DataCacheFactory(dataCacheFactoryConfiguration);
        }

        public override object Get(string key)
        {
            try
            {
                return this.retryPolicy.ExecuteAction<object>(() => this.DataCache.Get(key));
            }
            catch (DataCacheException ex)
            {
                cacheLogger.LogFatal(Name, "DataCacheException occurred", ex);
                return null;
            }
        }

        public override void Set(string key, object value)
        {
            try
            {
                this.retryPolicy.ExecuteAction(() => this.DataCache.Put(key, value));
            }
            catch (DataCacheException ex)
            {
                cacheLogger.LogFatal(Name, "DataCacheException occurred", ex);
            }
        }

        public override void Set(string key, object value, DateTime expiresAt)
        {
            Set(key, value, expiresAt.Subtract(DateTime.UtcNow));
        }

        public override void Set(string key, object value, TimeSpan validFor)
        {
            try
            {
                this.retryPolicy.ExecuteAction(() => this.DataCache.Put(key, value, validFor));
            }
            catch (DataCacheException ex)
            {
                cacheLogger.LogFatal(Name, "DataCacheException occurred", ex);
            }
        }

        public override bool Exists(string key)
        {
            try
            {
                return this.retryPolicy.ExecuteAction<bool>(() => (this.DataCache.Get(key) != null));
            }
            catch (DataCacheException ex)
            {
                cacheLogger.LogFatal(Name, "DataCacheException occurred", ex);
                return false;
            }
        }

        public override void Clear()
        {
            this.retryPolicy.ExecuteAction(() => this.DataCache.Clear());

            cacheLogger.LogInfo(Name, "Cache Cleared");
        }

        public override void Remove(string key)
        {
            this.retryPolicy.ExecuteAction(() => this.DataCache.Remove(key));
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
                if (dataCacheFactory != null)
                {
                    dataCacheFactory.Dispose();
                    dataCacheFactory = null;
                }
            }
        }

        internal static InRoleCache InitialiseFrom(InRoleCacheElement element, ICacheLogger logger)
        {
            DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration()
            {
                AutoDiscoverProperty = new DataCacheAutoDiscoverProperty(true, element.WebRole)
            };

            if (!element.Inherit)
            {
                configuration = new DataCacheFactoryConfiguration()
                {
                    MaxConnectionsToServer = 2,
                    IsCompressionEnabled = true,
                    AutoDiscoverProperty = new DataCacheAutoDiscoverProperty(true, element.WebRole),
                    TransportProperties = new DataCacheTransportProperties()
                    {
                        MaxBufferSize = element.MaxBufferSize,
                        MaxBufferPoolSize = element.MaxBufferPoolSize
                    },
                };

                if (element.LocalCacheEnabled)
                {
                    configuration.LocalCacheProperties = new DataCacheLocalCacheProperties(
                        element.LocalCacheObjectCount,
                        TimeSpan.FromSeconds(element.LocalCacheTimeoutValue),
                        (DataCacheLocalCacheInvalidationPolicy)Enum.Parse(typeof(DataCacheLocalCacheInvalidationPolicy), element.LocalCacheSync));
                    configuration.NotificationProperties = new DataCacheNotificationProperties(50, TimeSpan.FromSeconds(element.LocalCacheNotificationPollInterval));
                }
            }

            return new InRoleCache(element.Name, element.IsEnabled, configuration, logger);
        }
    }
}