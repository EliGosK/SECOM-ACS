using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace CSI.EPPlus
{
    [Serializable]
    public class ColumnItemStyle 
    {
        public ColumnItemStyle() { }
        public ColumnItemStyle(string styleName) 
            : this(styleName,null)
        {
        
        }

        public ColumnItemStyle(string styleName, string format)           
        {
            this.StyleName = styleName;
            this.Format = format;
        }

      

        [XmlAttribute]
        public string StyleName { get; set; }
        [XmlAttribute]
        public string Format { get; set; }
       
    }

    [Serializable]
    public class OutputColumnMapping
    {
        public OutputColumnMapping() 
        {
            this.ItemStyle = new ColumnItemStyle();
            this.Length = 0;
        }

        [XmlAttribute]
        public string ColumnName { get; set; }
        [XmlAttribute]
        public string DataField { get; set; }       
        [XmlAttribute]
        public bool RunningNo { get; set; }
        [XmlAttribute]
        public ColumnItemStyle ItemStyle { get; set; }
        [XmlAttribute]
        public ColumnItemStyle HeaderStyle { get; set; }
        [XmlAttribute]
        public int Length { get; set; }
        [XmlAttribute]
        public double Width { get; set; }

        public static OutputColumnMapping CreateRunningNoColumn(string columnName,double width, ColumnItemStyle itemStyle) 
        {
            return CreateRunningNoColumn(columnName, width, itemStyle, itemStyle);
        }

        public static OutputColumnMapping CreateRunningNoColumn(string columnName, double width, ColumnItemStyle headerStyle,ColumnItemStyle itemStyle)
        {
            return new OutputColumnMapping()
            {
                ColumnName = columnName,
                Width = width,
                ItemStyle = itemStyle,
                HeaderStyle = headerStyle,
                RunningNo = true
            };
        }

        public static OutputColumnMapping Create(string columnName, string dataField, double width, ColumnItemStyle itemStyle)
        {
            return Create(columnName, dataField, width, itemStyle, itemStyle);
        }

        public static OutputColumnMapping Create(string columnName, string dataField,double width, ColumnItemStyle headerStyle,ColumnItemStyle itemStyle)
        {
            return new OutputColumnMapping()
            {
                ColumnName = columnName,
                DataField = dataField,
                Width = width,
                ItemStyle = itemStyle,
                HeaderStyle = headerStyle
            };
        }

        public object GetValue<T>(T dataValue) 
        {
            if (String.IsNullOrEmpty(this.DataField)) { return null; }
            PropertyInfo prop = typeof(T).GetProperty(this.DataField);
            if (prop != null) 
            {
                string format = this.ItemStyle.Format;
                if (String.IsNullOrEmpty(this.ItemStyle.Format)) 
                {
                    format = prop.PropertyType == typeof(string) ? "{0}" : null;
                }

                if (String.IsNullOrEmpty(format))
                    return prop.GetValue(dataValue, null);
                else
                {
                    string value = String.Format(format, prop.GetValue(dataValue, null));
                    if (this.Length > 0) 
                    {
                        value = value.Substring(0, this.Length);
                    }
                    return value;
                    
                }
            }
            return null;
        }
    }

   
}
