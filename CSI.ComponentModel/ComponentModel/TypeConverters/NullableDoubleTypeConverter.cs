using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class NullableDoubleTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                if (String.IsNullOrEmpty(v)) { return null; }
                return Convert.ToDouble(value);
            }
            return ConvertFrom(context, culture, value);
        }
    }
}
