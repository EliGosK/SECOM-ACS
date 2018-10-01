using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Mail
{
    public class RazorMailOptions
    {
        public string BaseTemplateFolder { get; set; } = "Templates";
        public string EncodingText { get; set; } = "utf-8";

        public Encoding Encoding
        {
            get {
                return String.IsNullOrEmpty(this.EncodingText) ? System.Text.Encoding.Default : System.Text.Encoding.GetEncoding(this.EncodingText);
            }
        }
    }
}
