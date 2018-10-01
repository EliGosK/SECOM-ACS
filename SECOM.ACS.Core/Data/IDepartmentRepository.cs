using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IDepartmentRepository : IRepository<Department,string>
    {
        IEnumerable<Department> Find(DepartmentSearchCriteria criteria);
    }
}
