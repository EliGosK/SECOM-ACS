using System;
using System.Configuration;

namespace CSI.Configuration
{
    

    public class TypeConfigurationElement : ConfigurationElement
    {
        private const string typePropertyName = "type";

        public TypeConfigurationElement()
        {
        }

        public TypeConfigurationElement(string typeName)
        {
            this.TypeName = typeName;
        }

        public TypeConfigurationElement(System.Type type)
        {
            this.Type = type;
        }

        public System.Type Type
        {
            get
            {
                return System.Type.GetType(this.TypeName);
            }
            set
            {
                this.TypeName = value.AssemblyQualifiedName;
            }
        }

        [ConfigurationProperty("type", IsRequired=true, IsKey=true)]
        public string TypeName
        {
            get
            {
                return (string) base["type"];
            }
            set
            {
                base["type"] = value;
            }
        }
    }
}

