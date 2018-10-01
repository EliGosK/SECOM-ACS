using CSI.Text;
using CSI.Web.Mvc;
using ModelMetadataExtensions;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AcsItemOutViewModelResource))]
    [ModelBinder(typeof(AcsItemOutViewModelBinder))]
    public class AcsItemOutViewModel
    {
        public string RequestNo { get; set; }

        [Required]
        public string Status { get; set; } = SECOM.ACS.Models.RequestStatus.Requesting;
        
        //public string RequestStatus { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public string CreateBy { get; set; }

        public string UpdateBy { get; set; }

        public DateTime TakeOutDate { get; set; }

        public int AreaID { get; set; }

        [Required]
        public string Company { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string TakeOutName { get; set; }

        [Required]
        public int PurposeCodeID { get; set; }

        public string OtherPurpose { get; set; }
       
        public string Note { get; set; }

        // Superior Approval
        public System.Guid SuperiorApprovalID { get; set; }
        public string SuperiorApproveUserName { get; set; }
        public string SuperiorApprovalCode { get; set; }
        public DateTime? SuperiorApprovalDate { get; set; }
        public string SuperiorRejectReason { get; set; }
        public EmployeeInformationViewModel SuperiorEmployee { get; set; }

        // GA Approval
        public System.Guid GAApprovalID { get; set; }
        [Required]
        public string GAApproveUserName { get; set; }
        public string GAApprovalCode { get; set; }
        public DateTime? GAApprovalDate { get; set; }
        public string GARejectReason { get; set; }
        public EmployeeInformationViewModel GAEmployee { get; set; }

        public EmployeeInformationViewModel RequestEmployee { get; set; }
        public string RequestEmployeeName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.EmployeeName; } }
        public string RequestEmployeeDepartmentName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.Department; } }

        public RequestViewMode ViewMode { get; set; } = RequestViewMode.View;
        //public IList<AcsItemOutItemViewModel> Items { get; set; } = new List<AcsItemOutItemViewModel>();
        public AreaViewModel Area { get; set; }
        public MiscViewModel Purpose { get; set; }

        public string TempDataId { get; set; } = TextGenerator.Generate(32);
        public bool IsShowActualItem { get; set; } = false;

        public string ForceDoneBy { get; set; }
        public Nullable<System.DateTime> ForceDoneDate { get; set; }
        public EmployeeInformationViewModel ForceDoneEmployee { get; set; }

        public IList<AcsItemOutItemViewModel> AcsItemOutDetails { get; set; } = new List<AcsItemOutItemViewModel>();

        public bool AllowCancelRequest(IPrincipal user)
        {
            switch (this.Status)
            {
                case RequestStatus.Requesting:
                case RequestStatus.Approving:
                case RequestStatus.Approved:
                    return String.Compare(user.Identity.Name, this.CreateBy, true) == 0 && DateTime.Now.Date < this.TakeOutDate.Date;
                default:
                    return false;
            }
        }

        public bool AllowForceDone(IPrincipal user)
        {
            return this.Status == RequestStatus.Approved && DateTime.Now.Date > this.TakeOutDate.Date && user.Identity.GetUserData().IsVerifyItemOut;
        }
    }

    [MetadataConventions(ResourceType = typeof(AcsItemOutItemViewModelResource))]
    public class AcsItemOutItemViewModel
    {
        //public Guid DetailID { get; set; }

        public string DetailID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int ItemID { get; set; }

        public short Seq { get; set; }

        [Required]
        public int ItemTypeID { get; set; }

        [Required]
        public string ItemTypeName { get; set; }

        [StringLength(30)]
        [Required]
        public string ItemName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public bool Confidential { get; set; }

        [Required]
        public int RequestItemQty { get; set; }

        public DateTime? PlanReturnDate { get; set; }

        public int? ActualTakeOutQty { get; set; }

        public int? ActualReturnQty { get; set; }

        public DateTime? ActualReturn { get; set; }

        public string UpdateBy { get; set; }
    }

    public class AcsItemOutViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as AcsItemOutViewModel;
            if (model != null)
            {
                var state = new ModelState();

                if (bindingContext.ModelState.TryGetValue("AcsItemOutDetails", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["AcsItemOutDetails"] ?? "{}";
                        model.AcsItemOutDetails = JsonConvert.DeserializeObject<List<AcsItemOutItemViewModel>>(json, new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local });
                        state.Errors.Clear();
                    }
                }
            }
            return model;

        }
    }
}