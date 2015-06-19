using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class BlobCacheElement : CacheElement
    {
        [ConfigurationProperty("storageAccountKey", IsRequired = true)]
        public string StorageAccountKey
        {
            get
            {
                return (string)base["storageAccountKey"];
            }
        }

        [ConfigurationProperty("container", IsRequired = true)]
        public string Container
        {
            get
            {
                return (string)base["container"];
            }
        }
    }
}