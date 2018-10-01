using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class DepartmentRepository : EntityRepository<ACSContext, Department, string>, IDepartmentRepository
    {
        public DepartmentRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<Department> Find(DepartmentSearchCriteria criteria)
        {
            return Context.FindDepartment(criteria.DepartmentID, criteria.Name);
        }
    }
}
