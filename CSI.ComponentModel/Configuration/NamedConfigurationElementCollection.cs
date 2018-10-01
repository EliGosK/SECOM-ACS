namespace CSI.Configuration
{
    using System;
    using System.Configuration;

    public class NamedConfigurationElementCollection<T> : ConfigurationElementCollection where T: NamedConfigurationElement, new()
    {
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

        protected override object GetElementKey(ConfigurationElement element)
        {
            T local = (T) element;
            return local.Name;
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }
    }
}

