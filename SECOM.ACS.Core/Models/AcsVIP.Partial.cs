using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsVIP : IAcsRequest
    {
        public string DocumentType => AcsRequestTypeCodes.VIP;
        public string Requester => this.CreateBy;
        public EmployeeInformation RequestEmployee { get; set; }

        public string CardID { get; set; }
        public int[] Area { get; set; }
       
    }

    [Flags]
    public enum LoadAcsVIPOptions
    {
        None = 0,
        RequestEmployee = 1,
        Approval = 2,
        DefaultArea = 4,
        All = RequestEmployee | Approval | DefaultArea
    }
    
}
