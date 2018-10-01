using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public interface IDataInterfaceService
    {
        IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcs(string[] employees);
        IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsToAdd(DateTime sentDate, string[] transactions);
        IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsToCancel(string[] transactions);
        IEnumerable<EmployeeForImportAcs> GetEmployeesForImportAcsByEffectiveDate(DateTime effectiveDate);
        
    }
}
