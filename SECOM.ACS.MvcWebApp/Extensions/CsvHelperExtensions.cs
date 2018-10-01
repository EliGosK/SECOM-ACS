using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class CsvHelperExtensions
    {
        public static CsvPropertyMap Required(this CsvPropertyMap map, params string[] columnNames)
        {
            return map.Name(columnNames).ConvertUsing(row =>
            {
                foreach (var columnName in columnNames)
                {
                    string value;
                    if (row.TryGetField(columnName, out value))
                    {
                        if (String.IsNullOrEmpty(value)) {
                            throw new CsvParserException($"{columnName} is required");
                        }
                        return value;
                    }
                }
                return null;
            });

        }
    }
}