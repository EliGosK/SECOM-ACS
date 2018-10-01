using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class DateTypeConverter : TypeConverter
    {
        public DateTypeConverter()
            : this("en-US", "M/d/yyyy HH:mm:ss tt")
        {

        }

        public DateTypeConverter(string culture, params string[] format)
        {
            this.Culture = culture;
            this.Formats = format;
            this.DefaultDate = DateTime.MinValue;
        }

        public string Culture { get; set; }
        public string[] Formats { get; set; }
        public DateTime DefaultDate { get; set; }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                DateTime result = this.DefaultDate;
                // Custom Format
                if (DateTime.TryParseExact(v,this.Formats, CultureInfo.CreateSpecificCulture(this.Culture), DateTimeStyles.None, out result))
                {
                    return result;
                }

                if (DateTime.TryParse(v, CultureInfo.CreateSpecificCulture(this.Culture), DateTimeStyles.None, out result))
                {
                    return result;
                }
                return this.DefaultDate;
            }

            if (value.GetType() == typeof(DateTime))
            {
                return value;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
