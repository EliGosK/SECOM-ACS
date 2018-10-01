using System;
using System.Configuration;

namespace CSI.Configuration
{
    public abstract class ConfigurationElementCollectionBase<T> : ConfigurationElementCollection where T: ConfigurationElement, new()
    {
        protected ConfigurationElementCollectionBase()
        {
        }

        public void Add(T element)
        {
            base.BaseAdd(element, true);
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public bool Contains(string name)
        {
            return (base.BaseGet(name) != null);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return Activator.CreateInstance<T>();
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < base.Count; i++)
            {
                action(this.Get(i));
            }
        }

        public T Get(int index)
        {
            return (T) base.BaseGet(index);
        }

        public T Get(string name)
        {
            return (base.BaseGet(name) as T);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }
}

