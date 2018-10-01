using Microsoft.AspNet.Identity;
using SECOM.ACS.Identity.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Security;

namespace SECOM.ACS.Identity
{
    public class UserStore : IUserStore<User, int>,
        IUserRoleStore<User, int>,       
        IQueryableUserStore<User, int>,
        IUserPasswordStore<User, int>,
        IUserTokenProvider<User,int>
    {
        private readonly IdentityContext db;

        public UserStore(IdentityContext context)
        {
            this.db = context;
        }

        public IQueryable<User> Users
        {
            get { return this.db.Users; }
        }

        #region IUserStore

        public Task CreateAsync(User user)
        {
            this.db.Users.Add(user);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(User user)
        {
            this.db.Users.Remove(user);
            return this.db.SaveChangesAsync();
        }

        public Task<User> FindByIdAsync(int userId)
        {
            return this.db.Users               
                .FirstOrDefaultAsync(u => u.Id.Equals(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return this.db.Users
               .FirstOrDefaultAsync(u => String.Compare( u.UserName , userName,true)==0);
        }

        public Task UpdateAsync(User user)
        {
            this.db.Entry<User>(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }
        #endregion


        #region UserRole
        public Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            var role = this.db.Roles.SingleOrDefault(r => String.Compare(r.Name ,roleName,true)==0);

            if (role == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.RoleNotFound, new object[] { roleName }));
            }

            user.Roles.Add(role);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(user.Roles.Join(this.db.Roles, ur => ur.Id, r => r.Id, (ur, r) => r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            return
                Task.FromResult<bool>(
                    this.db.Roles.Any(r => r.Name == roleName && r.Users.Any(u => u.Id.Equals(user.Id))));
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            var role = user.Roles.Join(this.db.Roles, ur => ur.Id, r => r.Id, (ur, r) => r).Where(t => String.Compare(t.Name, roleName, true) == 0).FirstOrDefault();

            if (role != null)
            {
                user.Roles.Remove(role);
            }

            return Task.FromResult(0);
        }
        #endregion

        #region Password
        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.Password != null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordFormat = 1;
            user.Password = passwordHash;
            return Task.FromResult(0);
        }
        #endregion

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }
        }

        public Task<string> GenerateAsync(string purpose, UserManager<User, int> manager, User user)
        {
            var token = CSI.Text.TextGenerator.GenerateLowerCharacter(32);
            return Task.FromResult(token);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<User, int> manager, User user)
        {
            return Task.FromResult(true);
        }

        public Task NotifyAsync(string token, UserManager<User, int> manager, User user)
        {
            return null;
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<User, int> manager, User user)
        {
            return Task.FromResult(true);
        }
    }
}
