using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.TypeConversions
{
    public class NullableDateTimeConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public NullableDateTimeConverter(string[] formats, CultureInfo culture)
        {
            this.Formats = formats;
            this.Culture = culture;
        }

        public string[] Formats { get; private set; }
        public CultureInfo Culture { get; private set; }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text)) { return null; }

            DateTime value = DateTime.Now;
            if (DateTime.TryParseExact(text, this.Formats, this.Culture, System.Globalization.DateTimeStyles.None, out value))
            {
                return value;
            }
            return base.ConvertFromString(options, text);
        }
    }
}
