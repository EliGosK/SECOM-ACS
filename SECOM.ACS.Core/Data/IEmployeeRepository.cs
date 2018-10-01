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
    public interface IEmployeeRepository : IRepository<Employee,string>
    {
        IEnumerable<EmployeeApprovalDataView> GetSuperiorApprover(string user);
        IEnumerable<EmployeeApprovalDataView> GetAreaApprover(int areaId);
        IEnumerable<EmployeeApprovalDataView> GetGAApprover();
        IEnumerable<EmployeeApprovalDataView> GetAcknowledgePerson();

        Employee GetByUserName(string userName);
        IPagedList<Employee> GetPagedListByCriteria(EmployeeSearchCriteria criteria);
        EmployeeInformation GetInformation(string userNameOrEmployeeId);
        IEnumerable<EmployeeDataView> Find(EmployeeSearchCriteria criteria);
        IEnumerable<EmployeeDataView> GetEmployeeByCriteria(EmployeeSearchCriteria criteria);

        IEnumerable<UserRole> GetUserRolesByEmployeeID(string employeeId);        
        void UpdateRoleUserMapping(string employeeId, int[] roles);
        void UpdateAreaMapping(string employeeId, AreaMapping[] areaMappings);


        IEnumerable<EmployeeForImportAcs> GetForImportAcs(string[] employees);
        IEnumerable<EmployeeForImportAcs> GetForImportAcsToAdd(DateTime sentDate,string[] transactions);
        IEnumerable<EmployeeForImportAcs> GetForImportAcsToCancel(string[] transactions);
        IEnumerable<EmployeeForImportAcs> GetForImportAcsByEffectiveDate(DateTime effectiveDate);

        int CreateUser(Employee employee);
        void CreateEmployeeRoleMapping(int userId);
    }
}
