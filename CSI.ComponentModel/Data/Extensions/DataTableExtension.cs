using CSI.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CSI.Data.Extensions
{
    public static class DataTableExtension
    {
        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="dataTable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static T ToObject<T>(this DataTable dataTable) where T : class, new()
        {
            if (dataTable.Rows.Count > 0)
                return GetObject<T>(dataTable.Rows[0], DataColumnMappingManager.CreateMapping<T>());
            else
                return default(T);
        }

        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="dataTable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static T ToObject<T>(this DataTable dataTable, IList<DataColumnMapping> mappings) where T : new()
        {
            if (dataTable.Rows.Count > 0)
                return GetObject<T>(dataTable.Rows[0], mappings);
            else
                return new T();
        }

        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="datatable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static List<T> ToList<T>(this DataTable datatable) where T : new()
        {
            try
            {
                return datatable.AsEnumerable().ToList()
                    .ConvertAll<T>(row => GetObject<T>(row, DataColumnMappingManager.CreateMapping<T>()));
            }
            catch
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="datatable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static List<T> ToList<T>(this DataTable datatable, IList<DataColumnMapping> mappings)
            where T : new()
        {
            try
            {
                return datatable.AsEnumerable().ToList()
                    .ConvertAll<T>(row => GetObject<T>(row, mappings));
            }
            catch
            {
                return new List<T>();
            }
        }

        public static List<T> ToSingleList<T>(this DataTable datatable, string columnName) where T : class
        {
            try
            {
                var column = datatable.Columns[columnName];
                var columnIndex = column != null ? column.Ordinal : 0;
                return datatable.AsEnumerable().ToList()
                    .ConvertAll<T>(row => GetValue<T>(row, columnIndex));
            }
            catch
            {
                return new List<T>();
            }
        }

        public static List<T> ToSingleList<T>(this DataTable datatable, int columnIndex = 0) where T : class
        {
            try
            {
                return datatable.AsEnumerable().ToList()
                    .ConvertAll<T>(row => GetValue<T>(row, columnIndex));
            }
            catch
            {
                return new List<T>();
            }
        }

        public static T GetValue<T>(DataRow row, int columnIndex) where T : class
        {
            return (T)Convert.ChangeType(row[columnIndex], typeof(T));
        }

        //public static T GetObject<T>(DataRow row) 
        //    where T : new()
        //{
        //    return GetObject<T>(row, DataColumnMappingManager.CreateMapping<T>());
        //}

        public static T GetObject<T>(DataRow row, IList<DataColumnMapping> mappings)
            where T : new()
        {
            T obj = new T();
            object value;
            List<string> columnNames = new List<string>();
            foreach (DataColumn dataColumn in row.Table.Columns)
            {
                columnNames.Add(dataColumn.ColumnName);
            }

            foreach (PropertyInfo p in typeof(T).GetProperties())
            {
                var columnName = p.Name;
                // Looking TypeConverter from TypeConverterAttribute from property
                var attr = p.GetCustomAttribute(typeof(TypeConverterAttribute), true) as TypeConverterAttribute;
                var converterType = attr != null ? Type.GetType(attr.ConverterTypeName) : null;
                // Default TypeConverter for string type
                converterType = p.PropertyType == typeof(string) ? typeof(TrimStringTypeConverter) : null;

                var query = mappings.Where(t => String.Compare(p.Name, t.PropertyName, true) == 0).FirstOrDefault();
                if (query != null)
                {
                    // Override mapping from DataColumnMapping.
                    columnName = query.ColumnName;
                    converterType = query.TypeCoverter != null ? query.TypeCoverter : converterType;
                }

                try
                {
                    columnName = columnNames.Find(name => name.ToLower() == columnName.ToLower());
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        value = row[columnName];
                        if (value != null)
                        {
                            if (value == DBNull.Value) { value = String.Empty; }
                            if (Nullable.GetUnderlyingType(p.PropertyType) != null)
                            {
                                if (converterType == null)
                                    p.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(p.PropertyType).ToString())), null);
                                else
                                {
                                    var converter = Activator.CreateInstance(converterType) as TypeConverter;
                                    p.SetValue(obj, converter.ConvertFrom(value), null);
                                }
                            }
                            else
                            {
                                if (converterType == null)
                                    p.SetValue(obj, Convert.ChangeType(value, p.PropertyType), null);
                                else
                                {
                                    var converter = Activator.CreateInstance(converterType) as TypeConverter;
                                    p.SetValue(obj, converter.ConvertFrom(value), null);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // ignore error, Continue execute next property.
                }
            }
            return obj;
        }

        public static int ToBitAndValue(this int[] values)
        {
            if (values == null || values.Length == 0) { return 0; }
            var result = 0;
            foreach (var value in values)
            {
                result |= value;
            }
            return result;
        }
    }
}
