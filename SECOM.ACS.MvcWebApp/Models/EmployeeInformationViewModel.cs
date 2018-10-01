using CSI.Web.Mvc;
using ModelMetadataExtensions;
using Newtonsoft.Json;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(EmployeeInformationViewModelResource))]
    [ModelBinder(typeof(EmployeeInformationViewModelBinder))]
    public class EmployeeInformationViewModel
    {
        [Required]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public bool IsLending { get; set; }
        public bool IsVerifyItemIn { get; set; }
        public bool IsVerifyItemOut { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CardID { get; set; }
        public string PositionID { get; set; }
        public string DepartmentID { get; set; }
        public IList<RoleViewModel> UserGroups { get; set; } = new List<RoleViewModel>();
        public IList<AreaMappingViewModel> Areas { get; set; } = new List<AreaMappingViewModel>();
        public string UserGroupDataTempId { get; set; } = CSI.Text.TextGenerator.Generate(32);
        public string AreaMappingDataTempId { get; set; } = CSI.Text.TextGenerator.Generate(32);
    }

    public class EmployeeInformationViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as EmployeeInformationViewModel;
            if (model != null)
            {
                var state = new ModelState();
                if (bindingContext.ModelState.TryGetValue("Areas", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["Areas"] ?? "{}";
                        model.Areas = JsonConvert.DeserializeObject<List<AreaMappingViewModel>>(json);
                        state.Errors.Clear();
                    }
                }

                if (bindingContext.ModelState.TryGetValue("UserGroups", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["UserGroups"] ?? "{}";
                        model.UserGroups = JsonConvert.DeserializeObject<List<RoleViewModel>>(json);
                        state.Errors.Clear();
                    }
                }
            }
            return model;

        }
    }




}