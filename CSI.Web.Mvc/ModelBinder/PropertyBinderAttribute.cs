using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyBinderAttribute : Attribute
    {
        public PropertyBinderAttribute(Type type)
        {
            this.TypeConverter = type;
        }

        public Type TypeConverter { get; private set; }

        public bool BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var value = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);
            if (value != null)
            {
                var converter = Activator.CreateInstance(TypeConverter) as TypeConverter;
                var convertedValue = converter.ConvertFrom(value.AttemptedValue);
                propertyDescriptor.SetValue(bindingContext.Model, convertedValue);
            }
            return true;
        }
    }
}
