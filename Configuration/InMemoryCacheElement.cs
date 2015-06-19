using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class InMemoryCacheElement : CacheElement
    {
        [ConfigurationProperty("cacheMemoryLimitMegabytes", IsRequired = true)]
        public int CacheMemoryLimitMegabytes
        {
            get
            {
                return (int)base["cacheMemoryLimitMegabytes"];
            }
        }

        [ConfigurationProperty("physicalMemoryLimitPercentage", IsRequired = true)]
        public int PhysicalMemoryLimitPercentage
        {
            get
            {
                return (int)base["physicalMemoryLimitPercentage"];
            }
        }

        [ConfigurationProperty("memoryCheckPollingInterval", IsRequired = true)]
        public int MemoryCheckPollingInterval
        {
            get
            {
                return (int)base["memoryCheckPollingInterval"];
            }
        }
    }
}