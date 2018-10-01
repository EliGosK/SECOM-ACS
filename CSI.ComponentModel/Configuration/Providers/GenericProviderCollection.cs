namespace CSI.Configuration.Providers
{
    using System;
    using System.Configuration.Provider;
    using System.Reflection;

    public class GenericProviderCollection<T> : ProviderCollection where T: ProviderBase
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (!(provider is T))
            {
                throw new ArgumentException("Invalid provider type", "provider");
            }
            base.Add(provider);
        }

        public new T this[string name]
        {
            get
            {
                return (T) base[name];
            }
        }
    }
}

