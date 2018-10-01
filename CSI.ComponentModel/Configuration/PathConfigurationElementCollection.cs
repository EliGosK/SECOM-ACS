namespace CSI.Configuration
{
    using System;
    using System.Configuration;

    public class PathConfigurationElementCollection<T> : ConfigurationElementCollectionBase<T> where T: PathConfigurationElement, new()
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            T local = element as T;
            return local.Path;
        }
    }
}

