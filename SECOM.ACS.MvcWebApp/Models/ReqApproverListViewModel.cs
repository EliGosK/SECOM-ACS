using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class ReqApproverListViewModel
    {
        public System.Guid ApprovalID { get; set; }
        public string RequestType { get; set; }
        public string RequestNo { get; set; }
        public byte Step { get; set; }
        public string ApproveUserName { get; set; }
        
        public Nullable<int> AreaID { get; set; }
        public string ApprovalCode { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public string RejectReason { get; set; }


        public EmployeeViewModel ApproveEmployee { get; set; }
        public AreaViewModel Area { get; set; }
    }
}