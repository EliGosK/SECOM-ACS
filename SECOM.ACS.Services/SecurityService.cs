using CSI.ComponentModel;
using SECOM.ACS.Data;
using SECOM.ACS.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public partial class SecurityService : ISecurityService
    {
        protected IUnitOfWork CreateUnitOfWork()
        {
            var context = new ACSContext();
            context.Configuration.ProxyCreationEnabled = false;
            return new UnitOfWork(context);
        }

        public IEnumerable<PermissionMapping> GetPermissionRecords()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.PermissionRecords.GetAll();
            }
        }

        public IEnumerable<PermissionMapping> GetPermissionRecordsByRole(int roleId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.PermissionRecords.GetByRole(roleId);
            }
        }

        public Permission GetPermission(int roleId)
        {
            var dataView = new Permission() { RoleId = roleId };
            using (var unitOfWork = CreateUnitOfWork())
            {
                dataView.Permissions = unitOfWork.PermissionRecords.GetByRole(roleId).ToList();
                var d = unitOfWork.PermissionDashboards.Get(roleId);
                for (int i = 1; i <= 8; i++)
                {
                    var p = typeof(PermissionDashboard).GetProperty(String.Format("ViewDSH{0:00}", i));
                    if (p == null) { continue; }
                    if ((bool)p.GetValue(d, null))
                    {
                        dataView.DashboardAttributes.Add(String.Format("DSH{0:00}", i));
                    }
                }
                return dataView;
            }
        }

        public ObjectResult UpdatePermission(Permission dataView)
        {
            var permissions = dataView.Permissions;
            var roles = permissions.Select(t => t.RoleID).Distinct();

            using (var u = CreateUnitOfWork())
            {
                foreach (var role in roles)
                {
                    var currentPermission = GetPermissionRecordsByRole(role);
                    var newPermission = permissions.Where(t => t.RoleID == role);
                    // Add new permission
                    foreach (var p in newPermission)
                    {
                        var findItem = currentPermission.Where(t => t.ObjectID == p.ObjectID && t.RoleID == p.RoleID && String.Compare(t.PermissionName, p.PermissionName, true) == 0).FirstOrDefault();
                        if (findItem == null)
                        {
                            // Insert new permission
                            u.PermissionRecords.Add(p);
                        }
                    }

                    // Delete Exist Permission
                    foreach (var p in currentPermission)
                    {
                        var findItem = newPermission.Where(t => t.RoleID == p.RoleID && t.ObjectID == p.ObjectID && String.Compare(t.PermissionName, p.PermissionName,true)==0).FirstOrDefault();
                        if (findItem == null)
                        {
                            // Delete current permission
                            u.PermissionRecords.Remove(p);
                        }
                    }

                    // Update Dashboard Attributes
                    u.PermissionDashboards.Edit(dataView.ToPermissionDashboard());
                }
                u.Complete();
            }
            return ObjectResult.Succeed();
        }

        public ObjectResult DeletesPermissionRecord(int roleId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.PermissionRecords.Deletes(roleId);
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }

        public ObjectResult DeletesPermission(int roleId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.PermissionRecords.Deletes(roleId);
                unitOfWork.PermissionDashboards.Edit(new PermissionDashboard() { RoleId = roleId });
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }
    }
}
