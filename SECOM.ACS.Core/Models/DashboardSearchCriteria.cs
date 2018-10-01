using CSI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public class DashboardSearchCriteria : PagingOptions
    {
        public string User { get; set; }
        public string DocumentType { get; set; }
        public string Status { get; set; }
    }
}
