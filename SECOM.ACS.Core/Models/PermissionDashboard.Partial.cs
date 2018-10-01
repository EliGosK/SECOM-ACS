using CSI.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PermissionDashboard
    {
        //public int RoleId { get; set; }
        //public bool ViewDSH01 { get; set; }
        //public bool ViewDSH02 { get; set; }
        //public bool ViewDSH03 { get; set; }
        //public bool ViewDSH04 { get; set; }
        //public bool ViewDSH05 { get; set; }
        //public bool ViewDSH06 { get; set; }
        //public bool ViewDSH07 { get; set; }
        //public bool ViewDSH08 { get; set; }
        public string User { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
