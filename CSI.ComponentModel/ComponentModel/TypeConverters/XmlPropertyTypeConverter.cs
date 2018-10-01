namespace CSI.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlPropertyTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if ((sourceType != typeof(XmlNode)) && (sourceType != typeof(string)))
            {
                return base.CanConvertFrom(context, sourceType);
            }
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(XmlNode)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is XmlNode)
            {
                XmlNode node = (XmlNode) value;
                StringReader textReader = new StringReader(node.InnerXml);
                object obj2 = new XmlSerializer(context.PropertyDescriptor.PropertyType).Deserialize(textReader);
                textReader.Close();
                return obj2;
            }
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            if (!File.Exists((string) value))
            {
                XmlSerializer serializer = new XmlSerializer(context.PropertyDescriptor.PropertyType);
                StringReader reader2 = new StringReader((string) value);
                return serializer.Deserialize(reader2);
            }
            XmlSerializer serializer2 = new XmlSerializer(context.PropertyDescriptor.PropertyType);
            using (StreamReader reader3 = new StreamReader((string) value))
            {
                return serializer2.Deserialize(reader3);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object obj2;
            if (destinationType != typeof(XmlNode))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            StringWriter w = new StringWriter();
            XmlTextWriter writer2 = new XmlTextWriter(w);
            try
            {
                writer2.WriteStartElement("root");
                new XmlSerializer(value.GetType()).Serialize((XmlWriter) writer2, value);
                writer2.WriteEndElement();
                string xml = w.ToString();
                w.Close();
                writer2.Close();
                XmlDocument document = new XmlDocument();
                document.LoadXml(xml);
                obj2 = document.SelectSingleNode("/root");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                w.Close();
                writer2.Close();
            }
            return obj2;
        }
    }
}

