using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class NullableDateTypeConverter : TypeConverter
    {
        public NullableDateTypeConverter() : this("en-US", "yyyy-MM-dd", "yy/MM/dd")
        {

        }

        public NullableDateTypeConverter(string culture, params string[] format)
        {
            this.Culture = culture;
            this.Formats = format;
        }

        public string Culture { get; set; }
        public string[] Formats { get; set; }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                DateTime result = DateTime.MinValue;
                DateTime.TryParseExact(v, this.Formats, CultureInfo.CreateSpecificCulture(this.Culture), DateTimeStyles.None, out result);
                if (result == DateTime.MinValue)
                    return null;
                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
