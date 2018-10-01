using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class HostedProcessConfiguration
    {
        public string ServiceName { get; set; }
        public string ConfigFile { get; set; }
        public string LoggingFolder { get; set; } = "logging";
    }
}
