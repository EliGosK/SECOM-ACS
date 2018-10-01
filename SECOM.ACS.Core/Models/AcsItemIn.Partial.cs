using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsItemIn : IAcsRequest
    {
        public string DocumentType => AcsRequestTypeCodes.ItemIn;
        public string Requester => this.CreateBy;
        public EmployeeInformation RequestEmployee { get; set; }
        public Misc Purpose { get; set; }

        public Employee SuperiorApprovalEmployee { get; set; }
        public Employee AreaApprovalEmployee { get; set; }
        public Employee AcknowledgeEmployee { get; set; }
        public Employee ForceDoneEmployee { get; set; }
    }

    [Flags]
    public enum LoadAcsItemInOptions
    {
        None = 0,
        RequestEmployee = 1,
        Approval = 2,
        ItemInDetail = 4,
        Header = RequestEmployee | Approval,
        All = RequestEmployee | Approval | ItemInDetail
    }
}
