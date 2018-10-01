using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Framework.Identity
{
    public class IdentityUserClaim
    {
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
    }
}
