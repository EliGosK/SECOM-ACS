using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.TypeConversions
{
    public class AreaConverter : CsvHelper.TypeConversion.Int32Converter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            //int value = 0;
            //if (Int32.TryParse(text, out value))
            //{
            //    return value;
            //}
            //return 0;
            return String.IsNullOrWhiteSpace(text)?null:text.Trim();
        }
    }

}
