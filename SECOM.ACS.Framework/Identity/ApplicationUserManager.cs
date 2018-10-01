using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SECOM.ACS.Identity;


namespace SECOM.ACS.Identity
{
    public class ApplicationUserManager : UserManager<IdentityUser>
    {
        public ApplicationUserManager(IUserStore<IdentityUser> store)
            : base(store)
        {

        }

        public bool IgnoreCheckPassword { get; set; }
    }
}
