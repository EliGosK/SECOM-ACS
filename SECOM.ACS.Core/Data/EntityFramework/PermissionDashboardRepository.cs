using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class PermissionDashboardRepository : EntityRepository<ACSContext, PermissionDashboard,int>, IPermissionDashboardRepository
    {
        public PermissionDashboardRepository(ACSContext context) : base(context)
        {

        }

        public override PermissionDashboard Get(int id)
        {
            return Context.GetPermissionDashboard(id).FirstOrDefault();
        }

        public override void Edit(PermissionDashboard entity)
        {
            Context.UpdatePermissionDashboard(entity.RoleId, entity.ViewDSH01, entity.ViewDSH02, entity.ViewDSH03, entity.ViewDSH04, entity.ViewDSH05, entity.ViewDSH06, entity.ViewDSH07, entity.ViewDSH08, entity.User);
        }
    }
}
