using Microsoft.AspNet.Identity;
using SECOM.ACS.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity.EntityFramework
{
    public class RoleStore : IQueryableRoleStore<IdentityRole,int>
    {
        private readonly AccessControlContext context;

        public RoleStore(AccessControlContext context)
        {
            this.context = context;
        }


        public IQueryable<IdentityRole> Roles
        {
            get
            {
                return context.User
            }
        }

        public Task CreateAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var result = roleTable.Insert(role);
            return Task.FromResult(result);
        }

        public Task DeleteAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var result = roleTable.Delete(role);
            return Task.FromResult(result);
        }

        public void Dispose()
        {
           
        }

        public Task<IdentityRole> FindByIdAsync(int roleId)
        {
            var result = roleTable.GetRoleById(roleId);
            return Task.FromResult(result);
        }

        public Task<IdentityRole> FindByNameAsync(string roleName)
        {
            var result = roleTable.GetRoleByName(roleName);
            return Task.FromResult(result);
        }

        public Task UpdateAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var result = roleTable.Update(role);
            return Task.FromResult(result);
        }

       
    }
}
