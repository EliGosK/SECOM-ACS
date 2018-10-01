using System;
using System.Configuration;
using System.Reflection;

namespace CSI.Configuration
{
    public class TypeConfigurationElementCollection<T> : ConfigurationElementCollection where T: TypeConfigurationElement, new()
    {
        public void Add(T element)
        {
            base.BaseAdd(element, true);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public bool Contains(Type type)
        {
            return (base.BaseGet(type) != null);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return Activator.CreateInstance<T>();
        }

        public virtual T Get(object key)
        {
            if (key is Type)
            {
                return (base.BaseGet(key as Type) as T);
            }
            return default(T);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            T local = element as T;
            return local.Type.AssemblyQualifiedName;
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public T this[int index]
        {
            get
            {
                return (base.BaseGet(index) as T);
            }
        }
    }
}

