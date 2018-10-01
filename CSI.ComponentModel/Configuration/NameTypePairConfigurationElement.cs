namespace CSI.Configuration
{
    using System;
    using System.Configuration;

    public class NameTypePairConfigurationElement : TypeConfigurationElement
    {
        public const string nameProperty = "name";

        public NameTypePairConfigurationElement()
        {
        }

        public NameTypePairConfigurationElement(string name, string typeName) : base(typeName)
        {
            this.Name = name;
        }

        public NameTypePairConfigurationElement(string name, Type type) : base(type)
        {
            this.Name = name;
        }

        [ConfigurationProperty("name", IsKey=true, DefaultValue="Name", IsRequired=true), StringValidator(MinLength=1)]
        public string Name
        {
            get
            {
                return (string) base["name"];
            }
            set
            {
                base["name"] = value;
            }
        }
    }
}

