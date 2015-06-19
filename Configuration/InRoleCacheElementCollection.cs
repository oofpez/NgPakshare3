using System.Configuration;

namespace Wimt.CachingFramework.Configuration
{
    public class InRoleCacheElementCollection : ConfigurationElementCollection
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
                return "inRoleCache";
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new InRoleCacheElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((InRoleCacheElement)element).Name;
        }
    }
}