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
    public class Permission
    {
        public int RoleId { get; set; }
        public string User { get; set; }
        public IList<PermissionMapping> Permissions { get; set; } = new List<PermissionMapping>();
        public IList<string> DashboardAttributes { get; set; } = new List<string>();

        public PermissionDashboard ToPermissionDashboard()
        {
            var d = new PermissionDashboard()
            {
                RoleId = this.RoleId,
                User = this.User,
                ModifiedDate = DateTime.Now
            };

            foreach (var attr in DashboardAttributes)
            {
                var p = typeof(PermissionDashboard).GetProperty(String.Format("View{0}", attr));
                if (p == null) { continue; }
                p.SetValue(d, true);
            }
            return d;
        }
    }

    public partial class PermissionMapping 
    {
        public string Value
        {
            get {
                return String.Format("{0}_{1}", this.ObjectID, this.PermissionName);
            }
        }
    }

    public partial class PermissionRecordSearchCriteria
    {

    }

    public class PermissionRecordKey
    {
        public int RoleId { get; set; }
        public string ObjectId { get; set; }
        public string PermissionName { get; set; }
    }


    
}
