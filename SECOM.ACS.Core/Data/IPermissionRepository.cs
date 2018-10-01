using CSI.Core;
using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IPermissionRecordRepository : IRepository<PermissionMapping, PermissionRecordKey>
    {
        IEnumerable<PermissionMapping> GetByRole(int roleId);

        void Deletes(int roleId);
    }
}
