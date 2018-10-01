using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class CultureConfiguration
    {        
        public string DefaultCultureName { get; set; } = "en";

        public IList<LocalizeConfiguration> SupportCultures { get; set; } = new List<LocalizeConfiguration>();

        public string[] GetSupportCultures()
        {
            if (this.SupportCultures.Count() == 0) {
                var defaultCulture = CultureInfo.GetCultureInfo("en");
                this.SupportCultures.Add(new LocalizeConfiguration()
                {
                    CultureName = defaultCulture.Name,
                    LongDatePattern = defaultCulture.DateTimeFormat.LongDatePattern,
                    LongTimePattern = defaultCulture.DateTimeFormat.LongTimePattern,
                    ShortDatePattern = defaultCulture.DateTimeFormat.ShortDatePattern,
                    ShortTimePattern = defaultCulture.DateTimeFormat.ShortTimePattern,
                });
            }
            return this.SupportCultures.Select(t => t.CultureName).ToArray();
        }
        
    }

    public class LocalizeConfiguration
    {
        public string CultureName { get; set; }
        public string ShortDatePattern { get; set; }
        public string ShortTimePattern { get; set; }
        public string LongDatePattern { get; set; }
        public string LongTimePattern { get; set; }

        public CultureInfo ToCultureInfo()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo(this.CultureName).Clone() as CultureInfo;
            culture.DateTimeFormat.ShortDatePattern = this.ShortDatePattern;
            culture.DateTimeFormat.ShortTimePattern = this.ShortTimePattern;
            culture.DateTimeFormat.LongDatePattern = this.LongDatePattern;
            culture.DateTimeFormat.LongTimePattern = this.LongTimePattern;
            return culture;
        }
    }
}
