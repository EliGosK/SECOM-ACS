using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class AcsCsvConfiguration
    {
        public bool HasHeaderRecord { get; set; } = true;
        public string Delimiter { get; set; } = ",";
        public string DummyAccessGroup { get; set; } = "DUMMY";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
    }

    
}
