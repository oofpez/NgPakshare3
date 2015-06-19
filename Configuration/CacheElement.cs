using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public abstract class CacheElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base["name"];
            }
        }

        [ConfigurationProperty("enabled", IsRequired = true)]
        public bool IsEnabled
        {
            get
            {
                return (bool)base["enabled"];
            }
        }
    }
}