using Microsoft.AspNet.Identity;
using SECOM.ACS.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class AuthenticationConfiguration
    {
        public bool IgnoreCheckPassword { get; set; } = false;

        public bool AllowOnlyAlphanumericUserNames { get; set; } = false;
        public bool RequireUniqueEmail { get; set; } = false;
        public PasswordFormats PasswordFormat { get; set; } = PasswordFormats.Hash;
        public PasswordValidator PasswordValidator { get; set; } = new PasswordValidator()
        {
            RequiredLength = 6,
            RequireNonLetterOrDigit = false,
            RequireDigit = false,
            RequireLowercase = false,
            RequireUppercase = false
        };
    }
}
