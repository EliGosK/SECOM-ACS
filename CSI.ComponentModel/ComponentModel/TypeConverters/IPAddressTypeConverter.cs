namespace CSI.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net;

    public class IPAddressTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object data)
        {
            if ((data != null) && (data.GetType() == typeof(string)))
            {
                return IPAddress.Parse(data.ToString());
            }
            return base.ConvertFrom(context, culture, data);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value != null))
            {
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

