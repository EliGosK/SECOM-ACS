using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity
{
    public class UserRoleMapping
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int[] Roles { get; set; }
    }
}
