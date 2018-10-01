using System;
using System.Configuration;

namespace CSI.Configuration
{
   

    public class ProviderSettingsCollectionElement : ConfigurationElement
    {
        private const string defaultProviderNameProperty = "defaultProviderName";
        private const string providersProperty = "providers";

        [ConfigurationProperty("defaultProviderName", IsRequired=false)]
        public virtual string DefaultProviderName
        {
            get
            {
                return (string) base["defaultProviderName"];
            }
            set
            {
                base["defaultProviderName"] = value;
            }
        }

        [ConfigurationProperty("providers", IsRequired=true)]
        public ProviderSettingsCollection Providers
        {
            get
            {
                return (ProviderSettingsCollection) base["providers"];
            }
        }
    }
}

