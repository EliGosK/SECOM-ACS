using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.TypeConversions
{
    public class AcsDateTimeConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public AcsDateTimeConverter(DateTime defaultDate, string format) : base()
        {
            this.DefaultDate = defaultDate;
            this.Format = format;
        }

        public string Format { get; private set; } = "dd/MM/yyyy";
        public DateTime DefaultDate { get; private set; } = DateTime.MinValue;

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            if (this.CanConvertTo(typeof(string)))
            {
                if (value == null)
                {
                    return this.DefaultDate.ToString(this.Format);
                }
                DateTime dateValue = (DateTime)value;
                return dateValue.ToString(Format);
            }
            return base.ConvertToString(options, value);
        }
    }
}
