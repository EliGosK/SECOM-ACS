using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data
{
    public class DataColumnMappingManager
    {
        public static IList<DataColumnMapping> CreateMapping<T>()
        {
            var mappings = new List<DataColumnMapping>();
            foreach (var p in typeof(T).GetProperties())
            {
                var attrs = p.GetCustomAttributes(typeof(TypeConverterAttribute), true) as TypeConverterAttribute[];
                var columnAttrs = p.GetCustomAttributes(typeof(ColumnAttribute), true) as ColumnAttribute[];
                var columnName = columnAttrs != null && columnAttrs.Length > 0 ? columnAttrs[0].Name : p.Name;

                if (attrs != null && attrs.Length > 0)
                {
                    mappings.Add(new DataColumnMapping()
                    {
                        PropertyName = p.Name,
                        ColumnName = columnName,
                        TypeCoverter = Type.GetType(attrs[0].ConverterTypeName)
                    });
                }
                else
                {
                    mappings.Add(new DataColumnMapping()
                    {
                        PropertyName = p.Name,
                        ColumnName = columnName
                    });
                }
            }
            return mappings;
        }

        public void Build<T>() 
            where T : class
        {
            foreach (var property in typeof(T).GetProperties())
            {
                this.AddMapping<T>(property.Name, property.Name);
            }
        }

        public void AddMapping(string propertyName, string columnName, Type typeConverter) 
        {
            Mappings.Add(new DataColumnMapping() { PropertyName = propertyName, ColumnName = columnName, TypeCoverter = typeConverter });
        }

        public void AddMapping<T>(string propertyName, string columnName)
            where T : class
        {
            var p = typeof(T).GetProperty(propertyName); 
            var attrs = p.GetCustomAttributes(typeof(TypeConverterAttribute), true) as TypeConverterAttribute[];
            if (attrs != null && attrs.Length > 0)
            {
                Mappings.Add(new DataColumnMapping()
                {
                    PropertyName = propertyName,
                    ColumnName = columnName,
                    TypeCoverter = Type.GetType(attrs[0].ConverterTypeName)
                });
            }
            else {
                Mappings.Add(new DataColumnMapping()
                {
                    PropertyName = propertyName,
                    ColumnName = columnName
                });
            }
            
        }
        public IList<DataColumnMapping> Mappings { get; private set; } = new List<DataColumnMapping>();
    }

    public class DataColumnMapping
    {
        public DataColumnMapping() { }
        public DataColumnMapping(string propertyName,string columnName)
        {
            this.PropertyName = propertyName;
            this.ColumnName = columnName;
        }

        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public Type TypeCoverter { get; set; }
    }
}
