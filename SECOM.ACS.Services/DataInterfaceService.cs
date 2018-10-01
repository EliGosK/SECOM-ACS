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
    public class DataInterfaceService  : IDataInterfaceService
    {
        protected IUnitOfWork CreateUnitOfWork()
        {
            var context = new ACSContext();
            context.Configuration.ProxyCreationEnabled = false;
            return new UnitOfWork(context);
        }

        public IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcs(string[] employees)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.Employees.GetForImportAcs(employees);
            }
        }

        public IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsToAdd(DateTime sentDate, string[] transactions)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.Employees.GetForImportAcsToAdd(sentDate,transactions);
            }
        }

        public IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsToCancel(string[] transactions)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.Employees.GetForImportAcsToCancel(transactions);
            }
        }

        public IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsByEffectiveDate(DateTime effectiveDate)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.Employees.GetForImportAcsByEffectiveDate(effectiveDate);
            }
        }
        
    }
}
