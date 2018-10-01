using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class TimeTypeConverter : TypeConverter
    {
        public TimeTypeConverter()
            : this("en-US", "M/d/yyyy HH:mm:ss tt")
        {
            
        }

        public TimeTypeConverter(string culture, params string[] format)
        {
            this.Culture = culture;
            this.Formats = format;
        }

        public string Culture { get; set; }
        public string[] Formats { get; set; }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                var result = DateTime.MinValue;
                if (DateTime.TryParse(v, CultureInfo.CreateSpecificCulture(this.Culture), DateTimeStyles.None, out result))
                {
                    return new DateTime(1900, 1, 1, result.Hour, result.Minute, result.Second);
                }
                else {
                    return new DateTime(1900, 1, 1, 0, 0, 0);
                }              
            }

            if (value.GetType() == typeof(DateTime))
            {
                DateTime date = (DateTime)value;
                return new DateTime(1900, 1, 1, date.Hour, date.Minute, date.Second);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
