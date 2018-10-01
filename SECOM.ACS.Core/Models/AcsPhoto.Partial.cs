using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsPhoto : IAcsRequest
    {
        public Employee PhotoEmployee { get; set; }
        public Employee WitnessEmployee { get; set; }
        public EmployeeInformation RequestEmployee { get; set; }

        public Employee SuperiorApprovalEmployee { get; set; }
        public Employee AreaApprovalEmployee { get; set; }
        public Employee AcknowledgeEmployee { get; set; }
        public Employee ForceDoneEmployee { get; set; }

        public Misc Purpose { get; set; }

        public string DocumentType => AcsRequestTypeCodes.Photographing;
        public string Requester => this.CreateBy;
    }

    

    [Flags]
    public enum LoadAcsPhotoOptions
    {
        None = 0,
        PhotoEmployee = 1,
        PhotoWitness = 2,
        Purpose = 4,
        Area = 8,
        RequestEmployee = 16,
        WitnessEmployee = 32,
        Approval = 64,
        EquipItem = 128,
        Header = PhotoEmployee | PhotoWitness | Purpose | Area | RequestEmployee | WitnessEmployee | EquipItem,
        All = PhotoEmployee | PhotoWitness | Purpose | Area | RequestEmployee | WitnessEmployee | Approval | EquipItem
    }
}
