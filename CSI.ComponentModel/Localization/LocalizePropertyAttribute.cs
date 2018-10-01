using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Localization
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class LocalizePropertyAttribute : Attribute
    {
        public LocalizePropertyAttribute()
        {

        }

        public LocalizePropertyAttribute(string name, string propertyName) : this(name, "", propertyName)
        {

        }

        public LocalizePropertyAttribute(string name, string culture, string propertyName)
        {
            this.Name = name;
            this.Culture = culture;
            this.PropertyName = propertyName;
        }

        public string Name { get; private set; }
        public string Culture { get; private set; }
        public string PropertyName { get; private set; }
    }
}
