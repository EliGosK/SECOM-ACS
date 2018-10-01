namespace CSI.Configuration
{
    using CSI.ComponentModel.Design;
    using System;
    using System.Configuration;
    using System.Xml;

    public class PathConfigurationElement : ConfigurationElement
    {
        public const string pathProperty = "path";

        public PathConfigurationElement()
        {
        }

        public PathConfigurationElement(string path)
        {
            this.Path = path;
        }

        public void DeserializeElement(XmlReader reader)
        {
            base.DeserializeElement(reader, false);
        }

        [ResourceCategory(typeof(ResourceCategoryAttribute), "CategoryName"), ConfigurationProperty("path", IsKey=true, DefaultValue="Path", IsRequired=true), StringValidator(MinLength=1), ResourceDescription(typeof(ConfigurationDesignResource), "PathConfigurationElementNameDescription"), ResourceDisplayName(typeof(ConfigurationDesignResource), "PathConfigurationElementNameDisplayName")]
        public virtual string Path
        {
            get
            {
                return (string) base["path"];
            }
            set
            {
                base["path"] = value;
            }
        }
    }
}

