using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class InMemoryCacheElementCollection : ConfigurationElementCollection
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
                return "inMemoryCache";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new InMemoryCacheElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InMemoryCacheElement)element).Name;
        }
    }
}