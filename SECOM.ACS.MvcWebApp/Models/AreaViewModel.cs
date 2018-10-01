using CSI.Localization;
using CSI.Web.Mvc;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    [ModelBinder(typeof(AreaViewModelBinder))]
    [LocalizeProperty("AreaDisplay", "AreaDisplayEN")]
    [LocalizeProperty("AreaDisplay","th", "AreaDisplayTH")]
    public class AreaViewModel
    {
        [Required]
        public int AreaID { get; set; }

        [Required]
        [StringLength(50)]
        public string FactoryCode { get; set; }

        [Required]
        [StringLength(256)]
        public string AreaName { get; set; }

        [Required]
        [StringLength(256)]
        public string AreaDisplayEN { get; set; }

        [Required]
        [StringLength(256)]
        public string AreaDisplayTH { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfdtLevel { get; set; }

        [Required]
        public string Status { get; set; }

        public IList<GateViewModel> Gates { get; private set; } = new List<GateViewModel>();
        [Required]
        public string[] SelectedGates { get; set; }

        [Required]
        public string ApproverDepartment { get; set; }
        [Required]
        public string ApproverPosition { get; set; }

        public string Sub1Department { get; set; }
        public string Sub1Position { get; set; }

        public string Sub2Department { get; set; }
        public string Sub2Position { get; set; }

        public string AreaDisplay { get; set; }
    }

    public class AreaViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                            ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as AreaViewModel;
            if (model != null)
            {
                if (model.SelectedGates != null && model.SelectedGates.Length > 0 ) {
                    var q = from g in model.Gates
                            join sg in model.SelectedGates
                            on g.GateID equals sg
                            select g;

                    if (q.Count() != model.Gates.Count || q.Count() != model.SelectedGates.Length)
                    {
                        model.Gates.Clear();
                        foreach (var g in model.SelectedGates)
                        {
                            model.Gates.Add(new GateViewModel() { GateID = g });
                        }
                    }
                }
                    // Validate for AreaApprover 1
                    if (String.IsNullOrEmpty(model.ApproverPosition) || String.IsNullOrEmpty(model.ApproverDepartment))
                        bindingContext.ModelState.AddModelError("AreaApprover1", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Approver_Title));

                    // Validte for AreaApprover 2
                    if (!String.IsNullOrEmpty(model.Sub1Position) && String.IsNullOrEmpty(model.Sub1Department))
                        bindingContext.ModelState.AddModelError("AreaApprover2", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Substituter1_Title));

                    if (String.IsNullOrEmpty(model.Sub1Position) && !String.IsNullOrEmpty(model.Sub1Department))
                        bindingContext.ModelState.AddModelError("AreaApprover2", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Substituter1_Title));

                    // Validte for AreaApprover 3
                    if (!String.IsNullOrEmpty(model.Sub2Position) && String.IsNullOrEmpty(model.Sub2Department))
                        bindingContext.ModelState.AddModelError("AreaApprover2", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Substituter2_Title));

                    if (String.IsNullOrEmpty(model.Sub2Position) && !String.IsNullOrEmpty(model.Sub2Department))
                        bindingContext.ModelState.AddModelError("AreaApprover2", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Substituter2_Title));

                    if (!String.IsNullOrEmpty(model.Sub2Position) && !String.IsNullOrEmpty(model.Sub2Department)) {
                        if (String.IsNullOrEmpty(model.Sub1Position) || String.IsNullOrEmpty(model.Sub1Department))
                            bindingContext.ModelState.AddModelError("AreaApprover1", MessageHelper.InvalidInputAreaApprover(ViewResource.Area_AreaApprover_Substituter1_Title));
                    }
                
            }
            return model;

        }
    }
}