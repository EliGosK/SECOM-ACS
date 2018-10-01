using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsItemOut : IAcsRequest
    {
        public string DocumentType => AcsRequestTypeCodes.ItemOut;
        public string Requester => this.CreateBy;
        public EmployeeInformation RequestEmployee { get; set; }
        public Misc Purpose { get; set; }

        public EmployeeInformation SuperiorEmployee { get; set; }
        public EmployeeInformation GAEmployee { get; set; }
        public EmployeeInformation ForceDoneEmployee { get; set; }
    }

    [Flags]
    public enum LoadAcsItemOutOptions
    {
        None = 0,
        RequestEmployee = 1,
        Approval = 2,
        ItemOutDetail = 4,
        Header = RequestEmployee | Approval,
        All = RequestEmployee | Approval | ItemOutDetail
    }

    public partial class AcsItemOutDetailDataView
    {
        public Item Item { get; set; }
        public Misc ItemType { get; set; }
    }
}
