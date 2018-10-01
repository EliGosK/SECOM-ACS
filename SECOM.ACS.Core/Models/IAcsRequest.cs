using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAcsRequest
    {
        string ReqNo { get; set; }
        string Status { get; set; }
        string DocumentType { get;  }
        string Requester { get; }      
        string UpdateBy { get; set; }
        ICollection<ReqApproverList> ReqApproverList { get; set; }
        
    }

    
}
