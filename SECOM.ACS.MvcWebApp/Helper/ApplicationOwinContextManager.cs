using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Newtonsoft.Json;
using SECOM.ACS.Configuration;
using SECOM.ACS.Identity;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Mail;
using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class ApplicationOwinContextManager
    {
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var db = context.Get<IdentityContext>();
            //var manager = new ApplicationUserManager(new UserStore(db));
            var manager = new ApplicationUserManager(new UserStore(db));
            // We have to create our own user manager in order to override some default behavior:
            //
            //  - Override default password length requirement (6) with a length of 4
            //  - Override user name requirements to allow spaces and dots
            manager.UserValidator = new UserValidator<User, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = ApplicationContext.Setting.Authentication.AllowOnlyAlphanumericUserNames,
                RequireUniqueEmail = ApplicationContext.Setting.Authentication.RequireUniqueEmail
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = ApplicationContext.Setting.Authentication.PasswordValidator;

            var provider = new DpapiDataProtectionProvider("SECOM-ACS");
            manager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(provider.Create("PasswordReset"));
            // For Dev Only
            manager.IgnoreCheckPassword = ApplicationContext.Setting.Authentication.IgnoreCheckPassword;
            manager.StorePasswordFormat = ApplicationContext.Setting.Authentication.PasswordFormat;
            return manager;
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var db = context.Get<IdentityContext>();
            var manager = new ApplicationRoleManager(new RoleStore(db));
            return manager;
        }

      
    }
}
