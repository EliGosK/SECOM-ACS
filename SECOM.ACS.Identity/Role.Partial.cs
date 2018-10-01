using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity
{
    public partial class Role: IRole<int>
    {
        public Role(string name)
        {
            this.Name = name;
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
        }

        public int Id => this.RoleID;

        public bool ShouldSerializeUsers()
        {
            return false;
        }
    }
}
