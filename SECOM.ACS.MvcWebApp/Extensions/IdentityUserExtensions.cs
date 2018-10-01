using SECOM.ACS.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Principal;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class IdentityUserExtensions
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this User user, ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            // Add custom user claims here
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(user.GetUserData())));
            return userIdentity;
        }

        public static UserData GetUserData(this IIdentity user)
        {
            if (user.IsAuthenticated)
            {
                var identity = user as ClaimsIdentity;
                if (identity == null) { return UserData.Empty(); }
                var c = identity.Claims.FirstOrDefault(t => t.Type == ClaimTypes.UserData);
                if (c == null) { return UserData.Empty(); }
                return JsonConvert.DeserializeObject<UserData>(c.Value);
            }
            return UserData.Empty();
        }
    }
}