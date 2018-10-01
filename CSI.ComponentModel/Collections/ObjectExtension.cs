using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace CSI.Collections
{
    public static class ObjectExtension
    {
        public static Dictionary<string, string> ToDictionary(this object entity)
        {
            var results = new Dictionary<string, string>();
            foreach (var p in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = p.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                var name = attr == null ? p.Name : attr.DisplayName;

                var convertAttr = p.GetCustomAttribute(typeof(TypeConverterAttribute)) as TypeConverterAttribute;
                var value = p.GetValue(entity, null) ?? "";
                if (convertAttr != null)
                {
                    var typeConverter = Activator.CreateInstance(Type.GetType(convertAttr.ConverterTypeName)) as TypeConverter;
                    results.Add(name, typeConverter.ConvertToString(value));
                }
                else
                {
                    results.Add(name, value.ToString());
                }
            }
            return results;
            // .ToDictionary(prop => prop.Name, prop => prop.GetValue(entity, null));
        }
    }
}
