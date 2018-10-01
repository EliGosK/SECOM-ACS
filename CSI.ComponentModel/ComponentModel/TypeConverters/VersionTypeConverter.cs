namespace CSI.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class VersionTypeConverter : TypeConverter
    {
        public const string VersionRegEx = @"^((?<major>\d+)\.(?<minor>\d+))?(\.(?<build>\d+))?(\.(?<revision>\d+))?$";

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
            if ((data == null) || (data.GetType() != typeof(string)))
            {
                return base.ConvertFrom(context, culture, data);
            }
            Match match = Regex.Match((string) data, @"^((?<major>\d+)\.(?<minor>\d+))?(\.(?<build>\d+))?(\.(?<revision>\d+))?$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new Version(int.Parse(match.Groups["major"].Value), int.Parse(match.Groups["minor"].Value), int.Parse(match.Groups["build"].Value), int.Parse(match.Groups["revision"].Value));
            }
            return null;
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

