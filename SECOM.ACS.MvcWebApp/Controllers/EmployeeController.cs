using ClosedXML.Excel;
using CSI.Data.Extensions;
using CSI.Exceptions;
using CSI.Localization;
using CSI.Web.Mvc;
using CSI.Web.Mvc.KendoUI;
using CSI.Web.Mvc.KendoUI.Extensions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Mail.Models;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using SECOM.ACS.Tasks.ClassMaps;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class EmployeeController : AppControllerBase
    {
        private readonly IAccessControlService service;
        protected readonly ISecurityService securityService;

        public EmployeeController(IAccessControlService service, ISecurityService securityService)
        {
            this.service = service;
            this.securityService = securityService;
        }
        
        [ApplicationAuthorize("MAS050", PermissionNames.View)]
        [SiteMapPageTitle("MAS050")]
        public ActionResult Inquiry()
        {
            return View();
        }

        [ApplicationAuthorize("MAS051", PermissionNames.View)]
        [SiteMapPageTitle("MAS051")]
        public ActionResult Edit(string id)
        {
            var model = service.GetEmployeeInformation(id);
            if (model == null)
            {
                return View("_Message", MessageViewModel.Error(MessageHelper.DataNotFound("Employee data not found.")));
            }
            return View(model.ToViewModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApplicationSuspend]
        public ActionResult Edit(EmployeeInformationViewModel viewModel)
        {
            if (!ModelState.IsValid) {
                FlashMessage.Danger(MessageHelper.InvalidModelState(ModelState));
                return RedirectToAction("Edit", new { id = viewModel.EmployeeID });
            }

            var employee = service.GetEmployee(viewModel.EmployeeID);
            if (employee == null)
            {
                FlashMessage.Danger(MessageHelper.DataNotFound($"(Employee ID: {viewModel.EmployeeID})"));
                return RedirectToAction("Edit", new { id = viewModel.EmployeeID });
            }
            
            var entity = viewModel.ToEmployee();
            entity.CardID = employee.CardID;
            entity.UpdateBy = User.Identity.Name;
            var result = service.UpdateEmployee(entity);
            if (result.IsSucceed) {
                FlashMessage.Confirmation(MessageHelper.SaveCompleted());
                var options = ApplicationContext.Setting.Task.ToExportToAccessControlTaskOptions();
                options.TaskOptions.ExportModes = ExportToAccessControlModes.Employee;
                options.TaskOptions.Employees = new string[] { entity.EmpID };
                var taskResult = ExportInterfaceFileToAccessControl.Execute(options);
                if (!taskResult.IsSucceed)
                {
                    FlashMessage.Warning(MessageHelper.SaveFailed(taskResult.GetErrorMessage()));
                }

                UpdateSecurityContext();
                return RedirectToAction("Edit", new { id = viewModel.EmployeeID });
            }
            else {
                FlashMessage.Danger(MessageHelper.SaveFailed(result.GetErrorMessage()));
                return RedirectToAction("Edit", new { id = viewModel.EmployeeID });
            }
        }
        
        private void UpdateSecurityContext()
        {
            // Update Security Context (Authorize)
            try
            {
                var roles = RoleManager.Roles.Where(t => t.IsActive).ToList();
                var userRoles = UserManager.ToUserRoleMapping();
                var permissions = securityService.GetPermissionRecords();
                ApplicationContext.SecurityContext.AddData(roles, userRoles, permissions);
            }
            catch (Exception ex)
            {
                FlashMessage.Warning($"Update Security Cache failed. Error: {ExceptionUtility.GetLastExceptionMessage(ex)}");
            }
            
        }

        [HttpPost]
        public ActionResult List([DataSourceRequest]DataSourceRequest request, EmployeeSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            var dataItems = service.GetEmployeeDataViewsByCriteria(criteria);

            var result = dataItems.ToDataSourceResult(request, (EmployeeDataView employee)=> employee.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(string[] tempDataId)
        {
            foreach (var tempId in tempDataId)
            {
                ClearDataInCache(tempId);
            }
            return Ok("Clear data is successfully");
        }


        #region Temp User Group
        [HttpPost]
        public ActionResult GetTempUserGroupForCreate(string tempDataId, int[] roles)
        {
            var q = RoleManager.Roles;
            if (roles != null && roles.Length > 0) {
                q = q.Where(t => !roles.Contains(t.RoleID));
            }
            var findRoles = q.Select(t => new { RoleID = t.RoleID, Name = t.Name })
                .ToList();
            return JsonNet(findRoles, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        public ActionResult ListTempUserGroup([DataSourceRequest]DataSourceRequest request, string tempDataId, string employeeId)
        {
            var data = GetUserGroupDataFromCache(tempDataId, out bool cached).OrderBy(t => t.Name).ToList();
            if (!cached && !String.IsNullOrEmpty(tempDataId))
            {
                try
                {
                    data = service.GetUserRolesByEmployeeID(employeeId)
                        .Select(t => new RoleViewModel() { RoleID = t.RoleID, Name = t.Name, Description = t.Description })
                        .ToList();
                    SaveUserGroupDataIntoCache(tempDataId, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            return JsonNet(data.OrderBy(t=>t.Name), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateTempUserGroup(string tempDataId, int[] roles)
        {
            var data = GetUserGroupDataFromCache(tempDataId, out bool cached);
            foreach (var role in roles)
            {
                var findRole = RoleManager.FindByIdAsync(role).Result;
                if (findRole != null)
                {
                   
                    if (!data.Any(t => t.RoleID == role))
                    {
                        var dataToInserted = findRole.ToViewModel();
                        data.Add(dataToInserted);
                    }                   
                }
            }
            SaveUserGroupDataIntoCache(tempDataId, data);
            return Ok(MessageHelper.SaveCompleted());
        }

        [HttpPost]
        public ActionResult DeleteTempUserGroup(string tempDataId, RoleViewModel model)
        {
            var dataList = GetUserGroupDataFromCache(tempDataId, out bool cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.RoleID == model.RoleID);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveUserGroupDataIntoCache(tempDataId, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

      

        private List<RoleViewModel> GetUserGroupDataFromCache(string tempDataId, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<RoleViewModel>();
            }
            return cache[key] as List<RoleViewModel>;
        }

        private void SaveUserGroupDataIntoCache(string tempDataId, IList<RoleViewModel> model)
        {
            model.OrderBy(t => t.Name).ToList();
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(60),
                Priority = CacheItemPriority.Default
            };
            cache.Add(key, model, policy);
        }
        #endregion

        #region Temp Area Mapping
        [HttpPost]
        public ActionResult GetTempAreaMappingForCreate(string tempDataId, int[] areas)
        {
            var q = service.GetAreaDataViews(null,false);
            if (areas != null && areas.Length > 0) {
                q = q.Where(t => !areas.Contains(t.AreaID));
            }
            var dataItems = q.Select(t => new AreaDataViewModel { AreaID = t.AreaID, FactoryCode = t.FactoryCode, AreaDisplay = ModelLocalizeManager.GetValue(t, "AreaDisplay") })
                .ToList();
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        [HttpPost]
        public ActionResult ListTempAreaMapping(string tempDataId, string employeeId)
        {
            var data = GetAreaMappingDataFromCache(tempDataId, out bool cached)
                .OrderByDescending(t => t.IsMainArea ? 1 : 0)
                .ThenBy(t => t.AreaName).ToList();
            if (!cached && !String.IsNullOrEmpty(tempDataId))
            {
                try
                {
                    data = service.GetAreaMapping(employeeId).Select(t => t.ToViewModel()).ToList();
                    SaveAreaGroupDataIntoCache(tempDataId, data);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            return JsonNet(data.OrderByDescending(t => t.IsMainArea).ThenBy(t => t.Name), JsonRequestBehavior.AllowGet);
        }
              
        [HttpPost]
        public ActionResult CreateTempAreaMapping(string tempDataId, int[] areas)
        {
            var data = GetAreaMappingDataFromCache(tempDataId, out bool cached);

            foreach (var area in areas)
            {
                var findArea = service.GetArea(area);
                if (findArea != null)
                {
                    if (!data.Any(t => t.AreaID == area))
                    {
                        data.Add(findArea.ToMappingViewModel());
                    }                   
                }
            }
            SaveAreaGroupDataIntoCache(tempDataId, data);
            return Ok(MessageHelper.SaveCompleted());
        }

        public ActionResult DeleteTempAreaMapping(string tempDataId, AreaMappingViewModel model)
        {
            var dataList = GetAreaMappingDataFromCache(tempDataId, out bool cached);
            var dataToDeleted = dataList.FirstOrDefault(t => t.AreaID == model.AreaID);
            if (dataToDeleted != null)
            {
                dataList.Remove(dataToDeleted);
                SaveAreaGroupDataIntoCache(tempDataId, dataList);
                return JsonNet(model,JsonRequestBehavior.AllowGet);
            }
            return InternalServerError(MessageHelper.DeleteFailed());
        }

        private void ClearDataInCache(string tempDataId)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        private List<AreaMappingViewModel> GetAreaMappingDataFromCache(string tempDataId, out bool cached)
        {
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            cached = cache.Contains(key);
            if (!cache.Contains(key))
            {
                return new List<AreaMappingViewModel>();
            }
            return cache[key] as List<AreaMappingViewModel>;
        }

        private void SaveAreaGroupDataIntoCache(string tempDataId, IList<AreaMappingViewModel> model)
        {
            model.OrderByDescending(t => t.IsMainArea ? 1 : 0).ThenBy(t => t.AreaName).ToList();
            var cache = MemoryCache.Default;
            var key = tempDataId.ToLowerInvariant();
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
            var policy = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(60),
                Priority = CacheItemPriority.Default
            };
            cache.Add(key, model, policy);
        }
        #endregion

        [HttpPost]
        public ActionResult Find(FilterRequest filter,string employeeId = null,int pageSize = 20)
        {
            var criteria = new EmployeeSearchCriteria() { PageSize = pageSize };
            if (filter.Filters.Count > 0)
            {
                var value = filter.Filters[0].Value;
                criteria.EmployeeID = value;
                criteria.EmployeeName = value;               
            }

            var dataItems = service.FindEmployeeDataViews(criteria).ToList();
            if (!String.IsNullOrEmpty(employeeId))
            {
                var findEmployee = dataItems.FirstOrDefault(t => t.EmpID == employeeId);
                if (findEmployee == null)
                {
                    var selectedEmployee = service.GetEmployeeDataView(employeeId);
                    if (selectedEmployee != null)
                    {
                        dataItems.Insert(0,selectedEmployee);
                    }
                }
            }

            // Add parameter to filter employee who has card id.
            var result = dataItems.Select(t => new
            {
                EmployeeID = t.EmpID,
                EmployeeName = ModelLocalizeManager.GetValue(t, "EmpName"),
                EmployeeDisplay = String.Format("{0}: {1}", t.EmpID, ModelLocalizeManager.GetValue(t, "EmpName")),
                DepartmentName = ModelLocalizeManager.GetValue(t, "Department")
            }).OrderBy(t => t.EmployeeID);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FindEmployeesByEmployeeName(FilterRequest filter, int pageSize = 20)
        {
            var criteria = new EmployeeSearchCriteria() { PageSize = pageSize, Filter = EmployeeFilterTypes.EmployeeName };
            if (filter.Filters.Count > 0)
            {
                var value = filter.Filters[0].Value;
                criteria.EmployeeName = value;
            }

            var dataItem = service.FindEmployeeDataViews(criteria).ToList();
            var result = dataItem.Select(t => new
            {
                EmployeeID = t.EmpID,
                EmployeeName = ModelLocalizeManager.GetValue(t, "EmpName"),
                EmployeeDisplay = String.Format("{0}: {1}", t.EmpID, ModelLocalizeManager.GetValue(t, "EmpName")),
                DepartmentName = ModelLocalizeManager.GetValue(t, "Department")
            }).OrderBy(t => t.EmployeeID);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult FindEmployeesByEmployeeID(FilterRequest filter,int pageSize = 20)
        {
            var criteria = new EmployeeSearchCriteria() { PageSize = pageSize, Filter = EmployeeFilterTypes.EmployeeID };
            if (filter.Filters.Count > 0)
            {
                var value = filter.Filters[0].Value;
                criteria.EmployeeID = value;
            }

            var dataItem = service.FindEmployeeDataViews(criteria).ToList();
            var result = dataItem.Select(t => new
            {
                EmployeeID = t.EmpID,
                EmployeeName = ModelLocalizeManager.GetValue(t, "EmpName"),
                EmployeeDisplay = String.Format("{0}: {1}",t.EmpID, ModelLocalizeManager.GetValue(t, "EmpName")),
                DepartmentName = ModelLocalizeManager.GetValue(t, "Department")
            }).OrderBy(t => t.EmployeeID);
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [ApplicationAuthorize("MAS052", PermissionNames.View)]
        [SiteMapPageTitle("MAS052")]
        public ActionResult Import()
        {
            return View();
        }

        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var results = new Dictionary<string, List<EmployeeImportData>>();
            var configuration = new CsvConfiguration() { HasHeaderRecord = true };
            configuration.RegisterClassMap(new EmployeeImportDataMap());
            foreach (var file in files)
            {
                using (var workbook = new XLWorkbook(file.InputStream, XLEventTracking.Disabled))
                {
                    using (var reader = new CsvReader(new ExcelParser(workbook, configuration)))
                    {
                        var employeeToImports = reader.GetRecords<EmployeeImportData>().ToList();
                        results.Add(file.FileName, employeeToImports);
                    }
                }
            }
            return JsonNet(results.Select(t => new { fileName = t.Key, Data = t.Value }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ImportEmployee(EmployeeImportData[] employees)
        {
            var options = ApplicationContext.Setting.Task.ToUpdateEmployeeInfoTaskOptions();
            options.TaskOptions.EmployeeToImports = employees;
            var taskResult = UpdateEmployeeTask.Execute(options);
            if (taskResult.IsSucceed) {
                var dataItems = taskResult.DataState as List<EmployeeImportData>;
                if (dataItems == null)
                    return Ok(MessageHelper.SaveCompleted());

                var results = new
                {
                    Data = dataItems.Where(t => t.IsValid).ToArray(),
                    Errors = dataItems.Where(t => !t.IsValid).Select(t => new {
                        Data = t,
                        ErrorMessages = t.Errors.Select(e => ExceptionUtility.GetLastExceptionMessage(e)).ToArray()
                    })
                };
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                return JsonNet(results, settings, JsonRequestBehavior.AllowGet);
            }
            else {
                return InternalServerError(taskResult.GetErrorMessage());
            }
        }

        [ApplicationAuthorize("MAS051",PermissionNames.ResetPassword)]
        public async Task<ActionResult> ResetPassword(string employeeId)
        {
            var employee = service.GetEmployeeInformation(employeeId);
            if (employee == null) {
                return InternalServerError(MessageHelper.DataNotFound());
            }

            try
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(employee.UserID);
                var result = await UserManager.ResetPasswordAsync(employee.UserID, token, employee.EmpID);
                if (result.Succeeded)
                {
                    if (!String.IsNullOrEmpty(employee.Email))
                    {
                        MailManager.SendPasswordReset(new PasswordResetMailModel()
                        {
                            EmployeeID = employee.EmpID,
                            EmployeeNameEN = employee.EmpNameEN,
                            EmployeeNameTH = employee.EmpNameTH,
                            NewPassword = employee.EmpID
                        }, new string[] { employee.Email });
                    }
                    return Ok(MessageHelper.PasswordResetCompleted());
                }                
                return InternalServerError(result.Errors.First());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }
    }
}