using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace SECOM.ACS.Identity
{
    public class RoleStore : IQueryableRoleStore<Role,int>
    {
        private readonly IdentityContext db;

        public RoleStore(IdentityContext context)
        {
            this.db = context;
        }

        public IQueryable<Role> Roles
        {
            get
            {
                return db.Roles.AsQueryable();
            }
        }

        public Task CreateAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            this.db.Roles.Add(role);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            this.db.Roles.Remove(role);
            return this.db.SaveChangesAsync();
        }
       

        public Task<Role> FindByIdAsync(int roleId)
        {
            return this.db.Roles.FindAsync(roleId);
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            return this.db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            try
            {
                this.db.Roles.AddOrUpdate(role);
                return this.db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

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
    }
}
