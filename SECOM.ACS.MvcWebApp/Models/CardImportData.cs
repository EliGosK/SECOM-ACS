using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CsvHelper.TypeConversion;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class CardImportData
    {
        public string CardID { get; set; }
        public string CardType { get; set; }
        public string CardNo { get; set; }
        public string Note { get; set; }        
        public string[] AreaID { get; set; }
        public bool Active { get; set; }
    }

    public sealed class CardImportDataMap : CsvClassMap<CardImportData>
    {
        public CardImportDataMap()
        {
            Map(m => m.CardID).Name("CardID","Card ID");
            Map(m => m.CardType).Name("CardType", "Card Type");
            Map(m => m.CardNo).Name("CardNo", "Card No", "Card No."); ;
            Map(m => m.Note).Default("");
            Map(m => m.AreaID).Name("Area","Area ID").TypeConverter(new StringToStringArrayConverter());
            Map(m => m.Active).Name("Active","Status","IsActive").TypeConverter(new BooleanConverter());
        }
    }

    public class StringToStringArrayConverter : DefaultTypeConverter
    {
        public char Delimiter { get; private set; }

        public StringToStringArrayConverter(char delimiter = ',')
        {
            this.Delimiter = delimiter;
        }

        public override bool CanConvertFrom(Type type)
        {
            if (typeof(string) == type)
            {
                return true;
            }
            return base.CanConvertFrom(type);
        }

        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text)) { return new string[] { }; }
            return text.Split(new char[] { this.Delimiter });
        }

        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            string[] data = value as string[];
            if (data == null) return null;
            return String.Join(char.ToString(this.Delimiter), data);
            
        }
    }

    
}