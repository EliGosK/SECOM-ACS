namespace CSI.Configuration
{
    using System;
    using System.Configuration;

    public class DataProviderSection<T> : ConfigurationSection where T: ProviderSettingsCollectionElement
    {
        private const string dataPropertyName = "data";

        [ConfigurationProperty("data", IsRequired=true)]
        public T DataProvider
        {
            get
            {
                return (T) base["data"];
            }
        }
    }
}

