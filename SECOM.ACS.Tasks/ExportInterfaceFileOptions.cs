using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class ExportInterfaceFileOptions
    {
        public string TemplateFile { get; set; } = "acs-template.xls";

        public string TargetFileName { get; set; } = "acs-{0:yyyyMMdd_HHmmss}.xls";
        public string TargetFolder { get; set; } = "acs";

        public string DummyAccessGroup { get; set; } = "DUMMY";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public bool HasHeaderRecord { get; set; } = false;
        //public bool EnabledArchive { get; set; } = false;
        //public string ArchiveFileName { get; set; } = "acs_{0:yyyyMMdd_HHmm}.xls";
        //public string ArchiveFolder { get; set; } = "acs-history";

        //public string NetworkShareFolder { get; set; }
        //public bool EnabledNetworkShareAuthenticate { get; set; }
        //public string RemoteComputerName { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
    }

    public interface IExportInterfaceFileTaskOptions
    {
        ExportInterfaceFileOptions ExportInterfaceFileOptions { get; set; }
    }
}
