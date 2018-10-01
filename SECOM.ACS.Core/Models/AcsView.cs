using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{

    public class AcsView
    {
        public string DocumentType { get; set; }

        public string ReqNo { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string AreaId { get; set; }

        public string Status { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public Area Area { get; set; }
    }
}
