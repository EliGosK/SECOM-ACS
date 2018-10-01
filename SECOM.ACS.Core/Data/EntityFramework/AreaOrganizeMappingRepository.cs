using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AreaOrganizeMappingRepository : EntityRepository<ACSContext,AreaOrganizeMapping,Guid>, IAreaOrganizeMappingRepository
    {
        public AreaOrganizeMappingRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<AreaOrganizeMapping> GetBySearchCriteria(AreaOrganizeSearchCriteria criteria)
        {
            return Context.GetAreaOrganizeMappings(criteria.PositionID, criteria.DepartmentID, criteria.SpecialPositionID).ToList();
        }
    }
}
