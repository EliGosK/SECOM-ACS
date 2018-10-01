using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class PermissionRecordRepository : EntityRepository<ACSContext, PermissionMapping,PermissionRecordKey>, IPermissionRecordRepository
    {
        public PermissionRecordRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<PermissionMapping> GetByRole(int roleId)
        {
            return Context.PermissionMappings.Where(t => t.RoleID == roleId).ToList();
        }

        public void Deletes(int roleId)
        {
            Context.DeletePermissionMappingsByRoleId(roleId);
        }
    }
}
