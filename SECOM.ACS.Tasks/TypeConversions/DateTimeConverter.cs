using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.TypeConversions
{
    public class DateTimeConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public DateTimeConverter(string[] formats, CultureInfo culture,DateTime defaultDate)
        {
            this.Formats = formats;
            this.Culture = culture;
            this.DefaultDate = defaultDate;
        }

        public string[] Formats { get; private set; }
        public CultureInfo Culture { get; private set; }
        public DateTime DefaultDate { get; private set; }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text)) { return this.DefaultDate; }

            DateTime value = this.DefaultDate;
            if (DateTime.TryParseExact(text, this.Formats, this.Culture, System.Globalization.DateTimeStyles.None, out value))
            {
                return value;
            }
            return base.ConvertFromString(options, text);
        }
    }
}
