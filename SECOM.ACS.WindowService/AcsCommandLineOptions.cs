using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.WindowsService
{
    public class AcsCommandLineOptions
    {
        [Option('f',"configfile",DefaultValue = "appSetting.json",Required = false)]
        public string ConfigFile { get; set; }
    }
}
