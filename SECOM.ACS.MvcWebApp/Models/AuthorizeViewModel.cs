using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class PermissionViewModel
    {
        public int RoleId { get; set; }
        public IList<string> AuthorizeRules { get; set; } = new List<string>();
        public IList<string> DashboardAttributes { get; set; } = new List<string>();

        public Permission ToEntity(string user)
        {
            var permissions = new List<PermissionMapping>();
            foreach (var rule in this.AuthorizeRules)
            {
                var data = AuthorizeRule.Parse(rule);
                if (data == null) { continue; }
                permissions.Add(new PermissionMapping
                {
                    RoleID = this.RoleId,
                    ObjectID  = data.ObjectId,
                    PermissionName = data.PermissionName                    
                });
            }
            return new Permission()
            {
                RoleId = this.RoleId,
                User = user,
                DashboardAttributes = DashboardAttributes,
                Permissions = permissions
            }; 
        }
    }

    public class AuthorizeRule
    {
        public string ObjectId { get; set; }
        public string PermissionName { get; set; }

        public static AuthorizeRule Parse(string data)
        {
            var values = data.Split(new char[] { '_' });
            if (values.Length >= 2)
            {
                return new AuthorizeRule
                {
                    ObjectId = values[0],
                    PermissionName = values[1]
                };
            }
            return null;
        }
    }
}