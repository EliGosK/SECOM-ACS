using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data.Extensions
{
    public static class DatabaseResultExtensions
    {
        public static T ToObject<T>(this Dictionary<string, string> result)
            where T : new()
        {
            return GetObject<T>(result, DataColumnMappingManager.CreateMapping<T>());

        }

        public static T ToObject<T>(this Dictionary<string, string> result,IList<DataColumnMapping> mapping)
            where T : new()
        {
            return GetObject<T>(result, DataColumnMappingManager.CreateMapping<T>());
        }

        public static List<T> ToList<T>(this List<Dictionary<string, string>> results)
            where T : new()
        {
            var values = new List<T>();
            var mappings = DataColumnMappingManager.CreateMapping<T>();
            foreach (var result in results)
            {
                values.Add(GetObject<T>(result, mappings));
            }
            return values;
        }

        public static List<T> ToList<T>(this List<Dictionary<string, string>> results,IList<DataColumnMapping> mappings)
           where T : new()
        {
            var values = new List<T>();
            foreach (var result in results)
            {
                values.Add(GetObject<T>(result, mappings));
            }
            return values;
        }

        private static T GetObject<T>(Dictionary<string, string> result,IList<DataColumnMapping> mappings)
            where T : new()
        {
            T obj = new T();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var columnName = property.Name;
                var attr = property.GetCustomAttribute(typeof(TypeConverterAttribute), true) as TypeConverterAttribute;
                var converterTypeName = attr != null ? attr.ConverterTypeName : null;

                var query = mappings.Where(t => String.Compare(property.Name, t.PropertyName, true) == 0).FirstOrDefault();
                if (query!=null)
                {
                    columnName = query.ColumnName;
                    converterTypeName = query.TypeCoverter != null ? query.TypeCoverter.AssemblyQualifiedName : null;
                }

                var query2 = result.Where(t => String.Compare(t.Key, columnName, true) == 0)
                    .Select(t => t.Value);
                if (query2.Any())
                {
                    var value = query2.FirstOrDefault();                   
                    if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                    {
                        if (String.IsNullOrEmpty(converterTypeName))
                            property.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(property.PropertyType).ToString())), null);
                        else
                        {
                            var converter = Activator.CreateInstance(Type.GetType(converterTypeName)) as TypeConverter;
                            if (converter == null)
                                property.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(property.PropertyType).ToString())), null);
                            else
                                property.SetValue(obj, converter.ConvertFrom(value), null);
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(converterTypeName))
                            property.SetValue(obj, Convert.ChangeType(value, Type.GetType(property.PropertyType.ToString())), null);
                        else
                        {
                            var converter = Activator.CreateInstance(Type.GetType(converterTypeName)) as TypeConverter;
                            if (converter == null)
                                property.SetValue(obj, Convert.ChangeType(value, Type.GetType(property.PropertyType.ToString())), null);
                            else
                                property.SetValue(obj, converter.ConvertFrom(value), null);
                        }
                    }
                }
            }
            return obj;
        }

        public static void ThrowErrorOnNoRowAffected(this int rowAffected,string message)
        {
            if (rowAffected == 0)
            {
                throw new Exception(message);
            }
        }

    }

    
}
