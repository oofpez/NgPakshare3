using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class InRoleCacheElement : CacheElement
    {
        [ConfigurationProperty("webRole", IsRequired = true)]
        public string WebRole
        {
            get
            {
                return (string)base["webRole"];
            }
        }

        [ConfigurationProperty("inherit", IsRequired = false, DefaultValue = false)]
        public bool Inherit
        {
            get
            {
                return (bool)base["inherit"];
            }
        }

        [ConfigurationProperty("localCacheEnabled", IsRequired = false, DefaultValue = false)]
        public bool LocalCacheEnabled
        {
            get
            {
                return (bool)base["localCacheEnabled"];
            }
        }

        [ConfigurationProperty("localCacheSync", IsRequired = false, DefaultValue = "TimeoutBased")]
        public string LocalCacheSync
        {
            get
            {
                return (string)base["localCacheSync"];
            }
        }

        [ConfigurationProperty("localCacheObjectCount", IsRequired = false, DefaultValue = 10000)]
        public int LocalCacheObjectCount
        {
            get
            {
                return (int)base["localCacheObjectCount"];
            }
        }

        [ConfigurationProperty("localCacheTimeoutValue", IsRequired = false, DefaultValue = 300)]
        public int LocalCacheTimeoutValue
        {
            get
            {
                return (int)base["localCacheTimeoutValue"];
            }
        }

        [ConfigurationProperty("localCacheNotificationPollInterval", IsRequired = false)]
        public int LocalCacheNotificationPollInterval
        {
            get
            {
                return (int)base["localCacheNotificationPollInterval"];
            }
        }

        [ConfigurationProperty("maxBufferSize", IsRequired = false, DefaultValue = 0)]
        public int MaxBufferSize
        {
            get
            {
                return (int)base["maxBufferSize"];
            }
        }

        [ConfigurationProperty("maxBufferPoolSize", IsRequired = false, DefaultValue = 0)]
        public int MaxBufferPoolSize
        {
            get
            {
                return (int)base["maxBufferPoolSize"];
            }
        }
    }
}