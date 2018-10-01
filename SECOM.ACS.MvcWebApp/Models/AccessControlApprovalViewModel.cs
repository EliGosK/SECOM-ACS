using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [Obsolete("This class not used in project")]
    public class AccessControlApprovalViewModel
    {        
        public string EmployeeID { get; set; }
        public string ApprovalCode { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string RejectReason { get; set; }
    }

    public class ApprovalRequest
    {
        public string ApprovalID { get; set; }
        public string RequestNo { get; set; }
        public string ApprovalCode { get; set; }
        public string RejectReason { get; set; }
        
    }

}