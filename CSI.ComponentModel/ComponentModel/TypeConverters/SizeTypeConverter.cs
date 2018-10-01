namespace CSI.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class SizeTypeConverter : TypeConverter
    {
        public const string SizeRegEx = @"^((?<w>\d+),(?<h>\d+))$";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return Regex.IsMatch((string) context.Instance, @"^((?<w>\d+),(?<h>\d+))$", RegexOptions.IgnoreCase);
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object data)
        {
            if ((data == null) || (data.GetType() != typeof(string)))
            {
                return base.ConvertFrom(context, culture, data);
            }
            Match match = Regex.Match((string) data, @"^((?<w>\d+),(?<h>\d+))$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new Size(int.Parse(match.Groups["w"].Value), int.Parse(match.Groups["h"].Value));
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (((destinationType == typeof(string)) && (value != null)) && (value.GetType() == typeof(Size)))
            {
                Size size = (Size) value;
                return string.Format("{0},{1}", size.Width, size.Height);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

