namespace CSI.Configuration
{
    using System;
    using System.Configuration;

    public class NameTypePairConfigurationElementCollection<T> : TypeConfigurationElementCollection<T> where T: NameTypePairConfigurationElement, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return Activator.CreateInstance<T>();
        }

        public override T Get(object key)
        {
            if (key is string)
            {
                return (base.BaseGet(key as string) as T);
            }
            return (base.BaseGet(key.ToString()) as T);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            T local = (T) element;
            return local.Name;
        }
    }
}

