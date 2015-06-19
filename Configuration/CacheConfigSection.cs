using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class CacheConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("inRoleCaches", IsDefaultCollection = false)]
        public InRoleCacheElementCollection InRoleCaches
        {
            get
            {
                return (InRoleCacheElementCollection)base["inRoleCaches"];
            }
        }

        [ConfigurationProperty("inMemoryCaches", IsDefaultCollection = false)]
        public InMemoryCacheElementCollection InMemoryCaches
        {
            get
            {
                return (InMemoryCacheElementCollection)base["inMemoryCaches"];
            }
        }

        [ConfigurationProperty("blobCaches", IsDefaultCollection = false)]
        public BlobCacheElementCollection BlobCaches
        {
            get
            {
                return (BlobCacheElementCollection)base["blobCaches"];
            }
        }
    }
}