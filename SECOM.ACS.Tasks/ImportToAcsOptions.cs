using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class ImportToAcsOptions
    {
        public string TemplateFile { get; set; } = "acs-template.xls";

        public string TargetFileName { get; set; } = "acs.xls";
        public string TargetFolder { get; set; } = "acs";

        public bool EnabledArchive { get; set; } = false;
        public string HistoryFileName { get; set; } = "acs_{0:yyyyMMdd_HHmm}.xls";
        public string HistoryFolder { get; set; } = "acs-history";
    }
}
