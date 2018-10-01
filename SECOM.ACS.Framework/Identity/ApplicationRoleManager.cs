using SECOM.ACS.Identity;
using CSI.Exceptions;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Identity
{
    public class ApplicationRoleManager : RoleManager<IdentityRole, int>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, int> store)
            : base(store)
        {

        }

        public override Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            try
            {
                var findRole = base.FindByNameAsync(role.Name).Result;
                if (findRole != null)
                {
                    throw new Exception(String.Format("Create role fail. Role name '{0}' is duplicate.", role.Name));
                }
                return base.CreateAsync(role);
            }
            catch (Exception ex)
            {
                var result = IdentityResult.Failed(ExceptionUtility.GetLastExceptionMessage(ex));
                return Task.FromResult<IdentityResult>(result);
            }

        }

        public override Task<IdentityResult> UpdateAsync(IdentityRole role)
        {
            try
            {
                var findRole = base.FindByNameAsync(role.Name).Result;
                if (findRole != null && findRole.Id != role.Id)
                {
                    throw new Exception(String.Format("Update role fail. Role name '{0}' is duplicate.", role.Name));
                }
                return base.UpdateAsync(role);
            }
            catch (Exception ex)
            {
                var result = IdentityResult.Failed(ExceptionUtility.GetLastExceptionMessage(ex));
                return Task.FromResult<IdentityResult>(result);
            }
        }

        public override Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            try
            {
                var findRole = base.FindByIdAsync(role.Id);
                if (findRole == null)
                {
                    throw new Exception("Could not delete role. Role data not found.");
                }
                return base.DeleteAsync(role);
            }
            catch (Exception ex)
            {
                var result = IdentityResult.Failed(ExceptionUtility.GetLastExceptionMessage(ex));
                return Task.FromResult<IdentityResult>(result);
            }

        }
    }
}
