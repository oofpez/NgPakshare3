using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class BlobCacheElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "blobCache";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new BlobCacheElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BlobCacheElement)element).Name;
        }
    }
}