using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AreaApproverRepository : EntityRepository<ACSContext,AreaApprover, int>, IAreaApproverRepository
    {
        public AreaApproverRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<AreaApprover> GetBySearchCriteria(AreaApproverSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Department> GetDepartments()
        {
            return Context.GetDepartment().ToList();
        }
        public IEnumerable<Position> GetPositions()
        {
            return Context.GetPositionsForAreaApproval().ToList();
        }
    }
}
