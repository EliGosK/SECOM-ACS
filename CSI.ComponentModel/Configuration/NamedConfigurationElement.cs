namespace CSI.Configuration
{
    using CSI.ComponentModel.Design;
    using System;
    using System.Configuration;
    using System.Xml;

    public class NamedConfigurationElement : ConfigurationElement
    {
        public const string nameProperty = "name";

        public NamedConfigurationElement()
        {
        }

        public NamedConfigurationElement(string name)
        {
            this.Name = name;
        }

        public void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }

        [StringValidator(MinLength=1), ResourceDescription(typeof(ConfigurationDesignResource), "NamedConfigurationElementNameDescription"), ResourceDisplayName(typeof(ConfigurationDesignResource), "NamedConfigurationElementNameDisplayName"), ConfigurationProperty("name", IsKey=true, DefaultValue="Name", IsRequired=true), ResourceCategory(typeof(ResourceCategoryAttribute), "CategoryName")]
        public virtual string Name
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

