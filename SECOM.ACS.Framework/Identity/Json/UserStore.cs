using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity.Json
{
    public class UserStore : IUserStore<IdentityUser>,
        IUserLoginStore<IdentityUser>,
        IUserRoleStore<IdentityUser>,       
        IQueryableUserStore<IdentityUser>
       
    {
        private UserTable userTable;
        private RoleTable roleTable;
        private UserRolesTable userRolesTable;
        private UserLoginsTable userLoginsTable;

        private readonly string _folder;

        public IQueryable<IdentityUser> Users
        {
            get
            {
                return userTable.GetIdentityUsers().AsQueryable();
            }
        }

        public UserStore(string folder)
        {
            this._folder = folder;
            userTable = new UserTable(Path.Combine(folder,"users.json"));
            roleTable = new RoleTable(Path.Combine(folder, "roles.json"));
            userRolesTable = new UserRolesTable(Path.Combine(folder, "user-roles.json"));
            userLoginsTable = new UserLoginsTable(Path.Combine(folder, "user-logins.json"));
        }

        public Task AddToRoleAsync(IdentityUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            var role = roleTable.GetRoleByName(roleName);
            if (role!=null)
            {
                userRolesTable.Insert(user, role.Id);
            }

            return Task.FromResult<object>(null);
        }

        public Task CreateAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var result = userTable.Insert(user);
            return Task.FromResult(result);
        }

        public Task DeleteAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var result = userTable.Delete(user);
            return Task.FromResult(result);
        }

        public void Dispose()
        {
           
        }             

        public Task<IdentityUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }
            var user = userTable.GetUserById(userId);
            LoadUserRole(user);
            return Task.FromResult(user);
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }
            var user = userTable.GetUserByName(userName);
            LoadUserRole(user);
            return Task.FromResult(user);
        }       

        public Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var roles = userRolesTable.FindByUserId(user.UserName);
            {
                if (roles != null && roles.Count()>0)
                {
                    var roleNames = new List<string>();
                    foreach (var role in roles)
                    {
                        var r = roleTable.GetRoleById(role);
                        if (r != null) { roleNames.Add(r.Name); }
                    }
                    return Task.FromResult<IList<string>>(roleNames);
                }
            }
            return Task.FromResult<IList<string>>(new string[] { });
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("role");
            }

            var roles = userRolesTable.FindByUserId(user.Id);
            {
                if (roles != null && roles.Count()>0)
                {
                    foreach (var role in roles)
                    {
                        var r = roleTable.GetRoleById(role);
                        if (r == null) { continue; }

                        if (String.Compare(r.Name, roleName, true) == 0)
                        {
                            return Task.FromResult(true);
                        }
                    }                    
                }
            }
            return Task.FromResult(false);
        }      

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var result = userTable.Update(user);
            return Task.FromResult(result);
        }

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var user = userLoginsTable.GetBySId(login.ProviderKey);
            return Task.FromResult(user);
        }


        private async void LoadUserRole(IdentityUser user)
        {
            var roles = await GetRolesAsync(user);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }
            }
        }
    }
}
