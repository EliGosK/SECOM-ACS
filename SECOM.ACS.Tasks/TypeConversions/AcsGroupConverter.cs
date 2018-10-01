using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.TypeConversions
{
    public class AcsGroupConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public AcsGroupConverter() : this("DUMMY")
        {

        }

        public AcsGroupConverter(string defaultValue)
        {
            this.Default = defaultValue;
        }

        public string Default { get; set; }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            var group = value as string;
            return String.IsNullOrEmpty(group) ? this.Default : group;
        }
    }

   
}
