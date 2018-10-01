using CSI.Core;
using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class EmployeeRepository : EntityRepository<ACSContext,Employee, string>, IEmployeeRepository
    {
        public EmployeeRepository(ACSContext context) : base(context)
        {

        }

        public override void Edit(Employee entity)
        {
            if (String.IsNullOrEmpty(entity.CreateBy))
                Context.UpdateEmployee(entity.EmpID, entity.LendingFlag, entity.UpdateBy);
            else
                base.Edit(entity);
        }

        public IEnumerable<EmployeeApprovalDataView> GetAreaApprover(int areaId)
        {
            return Context.GetAreaApprover(areaId).ToList();
        }

        public IEnumerable<EmployeeApprovalDataView> GetSuperiorApprover(string user)
        {
            return Context.GetSuperiorApprover(user).ToList();
        }

        public IEnumerable<EmployeeApprovalDataView> GetGAApprover()
        {
            return Context.GetGAApprover().ToList();
        }

        public IEnumerable<EmployeeApprovalDataView> GetAcknowledgePerson()
        {
            return Context.GetAcknowledgePerson().ToList();
        }

        public Employee GetByUserName(string userName)
        {
            return Context.GetEmployeeByUserName(userName).FirstOrDefault();
        }

        public IPagedList<Employee> GetPagedListByCriteria(EmployeeSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public EmployeeInformation GetInformation(string userNameOrEmployeeId)
        {
            return Context.GetEmployeeInformation(userNameOrEmployeeId).FirstOrDefault();
        }

        public IEnumerable<EmployeeDataView> Find(EmployeeSearchCriteria criteria)
        {
            return Context.FindEmployee(criteria.EmployeeID, criteria.EmployeeName,criteria.Position,criteria.Department,(int)criteria.Filter)
                .Take(criteria.PageSize)
                .ToList();
        }

        public IEnumerable<EmployeeDataView> GetEmployeeByCriteria(EmployeeSearchCriteria criteria)
        {
            return Context.GetEmployeeDataViewsByCriteria(criteria.EmployeeID, criteria.EmployeeName, criteria.Department, criteria.Position).ToList();
        }

        public IEnumerable<UserRole> GetUserRolesByEmployeeID(string id)
        {
            return Context.GetUserRolesByEmployeeID(id).ToList();
        }

        public void UpdateRoleUserMapping(string employeeId, int[] roles)
        {
            Context.DeleteRoleUserMapping(employeeId);
            foreach (var role in roles)
            {
                Context.InsertRoleUserMapping(employeeId,role);
            }
        }

        public void UpdateAreaMapping(string employeeId, AreaMapping[] areaMappings)
        {
            Context.DeleteAreaMapping(employeeId);
            foreach (var areaMapping in areaMappings)
            {
                Context.InsertAreaMapping(employeeId, areaMapping.AreaID, areaMapping.IsMainArea);
            }
        }

        public IEnumerable<EmployeeForImportAcs> GetForImportAcs(params string[] employees)
        {
            if (employees == null || employees.Length == 0) { throw new ArgumentException("employe could not be null or empty array.", "employees"); }
            return Context.GetEmployeeForImportAcs(String.Join(",", employees)).ToList();
        }

        public IEnumerable<EmployeeForImportAcs> GetForImportAcsToAdd(DateTime sentDate,params string[] transactions)
        {
            if (transactions == null || transactions.Length == 0) { throw new ArgumentException("transactions could not be null or empty array.", "transactions"); }
            return Context.GetEmployeeForImportAcsToAdd(String.Join(",", transactions), sentDate).ToList();
        }

        public IEnumerable<EmployeeForImportAcs> GetForImportAcsToCancel(string[] transactions)
        {
            if (transactions == null || transactions.Length == 0) { throw new ArgumentException("transactions could not be null or empty array.", "transactions"); }
            return Context.GetEmployeeForImportAcsToCancel(String.Join(",", transactions)).ToList();
        }

        public IEnumerable<EmployeeForImportAcs> GetForImportAcsByEffectiveDate(DateTime effectiveDate)
        {
            return Context.GetEmployeeForImportAcsByEffectiveDate(effectiveDate).ToList();
        }

        public int CreateUser(Employee employee)
        {
            var result = Context.CreateUser(employee.EmpID, employee.CreateBy).FirstOrDefault();
            if (result != null) {
                return result.HasValue?result.Value:0;
            }
            return 0;
        }

        public void CreateEmployeeRoleMapping(int userId)
        {
            Context.CreateEmployeeRoleMapping(userId);
        }
    }
}
