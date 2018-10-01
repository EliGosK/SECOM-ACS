using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SECOM.ACS.Identity
{
    public class ApplicationUserManager : UserManager<User, int>
    {
        public ApplicationUserManager(IUserStore<User, int> store)
            : base(store)
        {

        }

        public bool IgnoreCheckPassword { get; set; }
        public PasswordFormats StorePasswordFormat { get; set; } = PasswordFormats.Hash;

        public override Task<User> FindAsync(string userName, string password)
        {
            var findUser = base.FindByNameAsync(userName).Result;
            if (findUser != null)
            {
                if (findUser.PasswordFormat == (int)PasswordFormats.Clear) {
                    if (findUser.Password == password)
                        return Task.FromResult(findUser);
                    else
                        return Task.FromResult<User>(null);
                }                    
            }
            return base.FindAsync(userName, password);
        }

        public IEnumerable<UserRoleMapping> ToUserRoleMapping()
        {
            var userRoles = new List<UserRoleMapping>();
            foreach (var user in Users)
            {
                var roles = user.Roles.Select(t => t.Id).ToArray();
                userRoles.Add(new UserRoleMapping() { UserId = user.Id, UserName = user.UserName, Roles = roles });
            }
            return userRoles;           
        }

        public override async Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = this.FindById(userId);
            if (user.PasswordFormat == (int)PasswordFormats.Clear)
            {
                if (String.Compare(currentPassword, user.Password) == 0)
                {
                    var validateResult = await this.PasswordValidator.ValidateAsync(newPassword);
                    if (validateResult.Succeeded)
                    {
                        user.PasswordFormat = (int)this.StorePasswordFormat;
                        user.Password = GetStorePassword(newPassword);
                        user.LastChangePassword = DateTime.Now;
                        return this.Update(user);
                    }
                    return validateResult;
                }
                return IdentityResult.Failed("Change password failed. Invalid password");
            }
            else
            {
                return await base.ChangePasswordAsync(userId, currentPassword, newPassword);               
            }
        }

        private string GetStorePassword(string password)
        {
            switch (this.StorePasswordFormat)
            {
                case PasswordFormats.Clear:
                    return password;
                default:
                    return this.PasswordHasher.HashPassword(password);
            }
        }
    }
}
