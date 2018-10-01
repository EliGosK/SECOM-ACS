using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class LocalizationConfiguration
    {
        public string DefaultCulture { get; set; } = "en";
        public string[] SupportCultures { get; set; } = new string[] { "en","th" };
    }
}
