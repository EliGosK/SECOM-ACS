using CSI.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public class DocumentTypes
    {
        public static readonly NameValueOption AccessEmployee = new NameValueOption("Employee", "Access Employee", "E");
        public static readonly NameValueOption ItemOut = new NameValueOption("ItemOut", "Item Out", "O");
        public static readonly NameValueOption ItemIn = new NameValueOption("ItemIn", "Item In", "I");
        public static readonly NameValueOption AccessVisitor = new NameValueOption("Visitor", "Access Visitor", "V");
        public static readonly NameValueOption Photographing = new NameValueOption("Photographing", "P");

        public static List<NameValueOption> GetList()
        {
            return new List<NameValueOption>
            {
                DocumentTypes.AccessEmployee, DocumentTypes.ItemOut, DocumentTypes.ItemIn, DocumentTypes.AccessVisitor, DocumentTypes.Photographing
            };
        }

        public static string GetKey(string value)
        {
            return GetList().Where(t => t.Value == value).Select(t => t.Key).FirstOrDefault();
        }

        public static string GetName(string value)
        {
            return GetList().Where(t => t.Value == value).Select(t => t.Name).FirstOrDefault();
        }
    }
}
