using CSI.Localization;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using System.Linq;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static partial class ModelExtensions
    {
        public static EmployeeViewModel ToViewModel(this Employee model)
        {
            var entity = AutoMapper.Mapper.Map<EmployeeViewModel>(model);
            return entity;
        }

        public static Employee ToEntity(this EmployeeViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<Employee>(model);
            return entity;
        }

        public static EmployeeApprovalDataViewModel ToViewModel(this EmployeeApprovalDataView employee)
        {
            var model = AutoMapper.Mapper.Map<EmployeeApprovalDataViewModel>(employee);
            return model;
        }

        public static EmployeeDataViewModel ToViewModel(this EmployeeDataView employee)
        {
            var model = AutoMapper.Mapper.Map<EmployeeDataViewModel>(employee);
            return model;
        }

        public static EmployeeInformationViewModel ToViewModel(this EmployeeInformation employee)
        {
            var viewModel = AutoMapper.Mapper.Map<EmployeeInformation, EmployeeInformationViewModel>(employee);
            viewModel.IsLending = employee.LendingFlag;
            viewModel.EmployeeName = ModelLocalizeManager.GetValue(employee, "EmpName");
            viewModel.Position = ModelLocalizeManager.GetValue(employee, "Position");
            viewModel.Department = ModelLocalizeManager.GetValue(employee, "Department");
            return viewModel;
        }

        public static EmployeeInformation ToEntity(this EmployeeInformationViewModel employee)
        {
            var entity = AutoMapper.Mapper.Map<EmployeeInformationViewModel, EmployeeInformation>(employee);
            entity.Roles = employee.UserGroups.Select(t => t.RoleID).ToArray();
            return entity;
        }

        public static Employee ToEmployee(this EmployeeInformationViewModel employee)
        {
            var entity = new Employee()
            {
                EmpID = employee.EmployeeID,
                LendingFlag = employee.IsLending,
                Email = employee.Email,
                CardID = employee.CardID,
            };
            entity.UserRoles = employee.UserGroups.Select(t => t.RoleID).ToArray();
            entity.AreaMappings = employee.Areas.Select(t => new AreaMapping() { AreaID = t.AreaID, IsMainArea = t.IsMainArea }).ToArray();
            return entity;
        }
    }
}