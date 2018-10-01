using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public class RequestSearchCriteria 
    {
        public string[] Status { get; set; }
        public int AreaID { get; set; }
        public string RequestBy { get; set; }
        public string[] DocumentTypes { get; set; }

        public void EnsureDataValid()
        {

        }
    }
    
}
