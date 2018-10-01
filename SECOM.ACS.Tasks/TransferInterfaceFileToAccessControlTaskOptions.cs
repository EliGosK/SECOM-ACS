using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class TransferInterfaceFileToAccessControlTaskOptions : ImportToAccessControlOptions
    {

    }

    public class ImportToAccessControlOptions
    {
        public string SourceFolder { get; set; } = "acs";

        public string TargetFileName { get; set; } = "acs.xlsx";
        public string TargetFolder { get; set; } = "acs-shared";

        public bool EnabledArchive { get; set; } = false;
        public string ArchiveFolder { get; set; } = "acs-history";

        public bool EnabledNetworkShareAuthenticate { get; set; }
        public string RemoteComputerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    
}
