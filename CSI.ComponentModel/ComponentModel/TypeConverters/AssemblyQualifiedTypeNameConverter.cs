using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace CSI.ComponentModel
{
    public class AssemblyQualifiedTypeNameConverter : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = (string) value;
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            Type type = Type.GetType(str, false);
            if (type == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, CSI.Properties.Resources.ExceptionInvalidType, new object[] { str }));
            }
            return type;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                Type type = value as Type;
                if (type == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, CSI.Properties.Resources.ExceptionCanNotConvertType, new object[] { typeof(Type).Name }));
                }
                if (type != null)
                {
                    return type.AssemblyQualifiedName;
                }
            }
            return null;
        }
    }
}

