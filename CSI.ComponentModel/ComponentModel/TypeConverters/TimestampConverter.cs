using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CSI.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataConverterAttribute : Attribute
    {
        public DataConverterAttribute(TypeConverter converter)
        {
            this.Converter = converter;
        }

        public DataConverterAttribute(Type converterType)
        {
            this.Converter = Activator.CreateInstance(converterType) as TypeConverter;
        }

        public TypeConverter Converter { get; private set; }
    }

    public class TimeStringConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string v = (string)value;
                string[] values = v.Split('.', ':');
                switch (values.Length)
                {
                    case 3:
                        return new DateTime(1900, 1, 1, Int32.Parse(values[0]), Int32.Parse(values[1]), Int32.Parse(values[2]));
                    case 2:
                        return new DateTime(1900, 1, 1, Int32.Parse(values[0]), Int32.Parse(values[1]), 0);
                    default:
                        return new DateTime(1900, 1, 1, 0, 0, 0);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class DateTimeConverter : TypeConverter
    {
        public DateTimeConverter() 
            : this("en-US","yyyy-MM-dd","yy/MM/dd")
        {

        }

        public DateTimeConverter(string culture,params string[] format) 
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
                DateTime.TryParseExact(v, this.Formats, CultureInfo.CreateSpecificCulture(this.Culture), DateTimeStyles.None, out result);
                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class NullableDateTimeConverter : TypeConverter
    { 
        public NullableDateTimeConverter() : this("en-US","yyyy-MM-dd","yy/MM/dd")
        {

        }

        public NullableDateTimeConverter(string culture, params string[] format) 
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

    

    public class NullableDoubleConverter : TypeConverter
    {
        public NullableDoubleConverter()
        {

        }

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

    public class NullableIntegerConverter : TypeConverter
    {
        public NullableIntegerConverter()
        {

        }

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
                return Convert.ToInt32(value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class PointConverter : TypeConverter
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
                int result = 0;
                Int32.TryParse(v, out result);
                return result;
            }
            return ConvertFrom(context, culture, value);
        }
    }
}
