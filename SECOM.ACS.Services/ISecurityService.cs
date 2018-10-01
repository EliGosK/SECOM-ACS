using CSI.ComponentModel;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public interface ISecurityService
    {
        IEnumerable<PermissionMapping> GetPermissionRecords();
        IEnumerable<PermissionMapping> GetPermissionRecordsByRole(int roleId);

        Permission GetPermission(int roleId);
        ObjectResult UpdatePermission(Permission permission);
        ObjectResult DeletesPermissionRecord(int roleId);

        ObjectResult DeletesPermission(int roleId);
    }
}
