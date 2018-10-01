using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace CSI.ComponentModel
{
    

    public class TimeSpanTypeConverter : TypeConverter
    {
        public const string TimeSpanRegEx = @"^((?<hours>\d{1,2}):(?<minutes>\d{1,2}))$";
        public const string TimeSpanWithOutColonRegEx = @"^((?<hours>\d{1,2})(?<minutes>\d{1,2}))$";

        public TimeSpanTypeConverter() : this(false)
        {
        }

        public TimeSpanTypeConverter(bool hasColonDelimiter)
        {
            this.HasColonDelimiter = hasColonDelimiter;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return Regex.IsMatch((string) context.Instance, this.HasColonDelimiter ? @"^((?<hours>\d{1,2}):(?<minutes>\d{1,2}))$" : @"^((?<hours>\d{1,2})(?<minutes>\d{1,2}))$", RegexOptions.IgnoreCase);
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
            Match match = Regex.Match((string) data, this.HasColonDelimiter ? @"^((?<hours>\d{1,2}):(?<minutes>\d{1,2}))$" : @"^((?<hours>\d{1,2})(?<minutes>\d{1,2}))$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new TimeSpan(int.Parse(match.Groups["hours"].Value), int.Parse(match.Groups["minutes"].Value), 0);
            }
            return TimeSpan.MinValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (((destinationType != typeof(string)) || (value == null)) || (value.GetType() != typeof(TimeSpan)))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            TimeSpan span = (TimeSpan) value;
            if (this.HasColonDelimiter)
            {
                return string.Format("{0:00}:{1:00}", span.Hours, span.Minutes);
            }
            return string.Format("{0:00}{1:00}", span.Hours, span.Minutes);
        }

        public bool HasColonDelimiter { get; private set; }
    }
}

