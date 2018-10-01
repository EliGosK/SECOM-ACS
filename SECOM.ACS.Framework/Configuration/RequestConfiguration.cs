using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class RequestConfiguration
    {
        public TimeSpan DefaultEntryTimeFrom { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan DefaultEntryTimeTo { get; set; } = new TimeSpan(18, 0, 0);

        
        
    }
}
