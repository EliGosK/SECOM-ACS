using CSI.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web.Hosting;

namespace CSI.ComponentModel
{
    public class FileSystemInfoTypeConverter : TypeConverter
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
            string str2;
            if ((data == null) || (data.GetType() != typeof(string)))
            {
                return base.ConvertFrom(context, culture, data);
            }
            string path = data.ToString();
            if (Path.IsPathRooted(path))
            {
                str2 = path;
            }
            else
            {
                str2 = HostingEnvironment.MapPath(path);
            }
            FileSystemInfo fileSystemInfo = FileUtility.GetFileSystemInfo(str2);
            if (fileSystemInfo == null)
            {
                throw new Exception(string.Format(null, "Could not resolve path '{0}' to a file or directory.", new object[] { str2 }));
            }
            return fileSystemInfo;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (value != null))
            {
                value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

