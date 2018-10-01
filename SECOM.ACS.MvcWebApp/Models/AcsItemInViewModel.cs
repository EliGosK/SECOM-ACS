using CSI.Text;
using CSI.Web.Mvc;
using ModelMetadataExtensions;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AcsItemInViewModelResource))]
    [ModelBinder(typeof(AcsItemInViewModelBinder))]
    public class AcsItemInViewModel
    {
        public string ReqNo { get; set; }
        public string Status { get; set; }
        public System.DateTime TakeInDate { get; set; }
        [Required]
        public int AreaID { get; set; }
        [Required]
        public string DeptName { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public string TakeInName { get; set; }
        [Required]
        public int PurposeCodeID { get; set; }
        [Required]
        public string OtherPurpose { get; set; }
        public string Note { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public string ForceDoneBy { get; set; }
        public Nullable<System.DateTime> ForceDoneDate { get; set; }

        public virtual IList<AcsItemInDetailViewModel> AcsItemInDetails { get; set; } = new List<AcsItemInDetailViewModel>();

        public bool Mode { get; set; } = false;

       // public string ViewMode { get; set; }
        public RequestViewMode ViewMode { get; set; } = RequestViewMode.View;
        public string TempDataId { get; set; } = TextGenerator.Generate(32);

        public AreaViewModel Area { get; set; }
        public MiscViewModel Purpose { get; set; }


        // Superior Approval
        public Guid SuperiorApprovalID { get; set; }
        public string SuperiorApproveUserName { get; set; }
        public string SuperiorApprovalCode { get; set; }
        public DateTime? SuperiorApprovalDate { get; set; }
        public string SuperiorRejectReason { get; set; }
        public EmployeeViewModel SuperiorApprovalEmployee { get; set; }

        // Area Approval        
        public Guid AreaApprovalID { get; set; }
        [Required]
        public string AreaApproveUserName { get; set; }
        public string AreaApprovalCode { get; set; }
        public DateTime? AreaApprovalDate { get; set; }
        public string AreaRejectReason { get; set; }
        public EmployeeViewModel AreaApprovalEmployee { get; set; }

        // Acknowledge Approval
        public Guid AcknowledgeApprovalID { get; set; }
        [Required]
        public string AcknowledgeUserName { get; set; }
        public DateTime? AcknowledgeDate { get; set; }
        public EmployeeViewModel AcknowledgeEmployee { get; set; }

        public EmployeeInformationViewModel RequestEmployee { get; set; }
        public EmployeeViewModel ForceDoneEmployee { get; set; }

        public string RequestEmployeeName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.EmployeeName; } }
        public string RequestEmployeeDepartmentName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.Department; } }

        public bool AllowCancelRequest(IPrincipal user)
        {
            switch (this.Status)
            {
                case RequestStatus.Requesting:
                case RequestStatus.Approving:
                case RequestStatus.Approved:
                    return String.Compare(user.Identity.Name, this.CreateBy, true) == 0 && DateTime.Now.Date < this.TakeInDate.Date;
                default:
                    return false;
            }
        }
      
        public bool AllowForceDone(IPrincipal user)
        {
            return this.Status == RequestStatus.Approved && DateTime.Now.Date > this.TakeInDate.Date && user.Identity.GetUserData().IsVerifyItemIn;
        }

        public bool AllowForActualTakeInQty(IPrincipal user)
        {
            return DateTime.Now.Date == this.TakeInDate.Date && this.Status == RequestStatus.Approved;
        }

        public bool AllowForActualReturnQty(IPrincipal user)
        {
            return DateTime.Now.Date > this.TakeInDate.Date && this.Status == RequestStatus.Approved;
        }

        public bool AllowForActualReturnDate(IPrincipal user)
        {
            return DateTime.Now.Date > this.TakeInDate.Date && this.Status == RequestStatus.Approved;
        }
    }

    [MetadataConventions(ResourceType = typeof(AcsItemInDetailViewModelResource))]
    public class AcsItemInDetailViewModel
    {
        public Guid DetailID { get; set; } = Guid.NewGuid();
        [Required]
        public string ReqNo { get; set; }
        public short Seq { get; set; }
        [Required]
        public int ItemID { get; set; }
        [Required]
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public bool Confidential { get; set; }
        public int RequestItemQty { get; set; }
        public DateTime? PlanReturnDate { get; set; }
        [UIHint("ActualItemQty")]
        public int? ActualTakeInQty { get; set; }
        [UIHint("ActualItemQty")]
        public int? ActualReturnQty { get; set; }
        [UIHint("ActualItemReturn")]
        public DateTime? ActualReturnDate { get; set; }
        public string UpdateBy { get; set; }
      
    }
    public class AcsItemInViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as AcsItemInViewModel;
            if (model != null)
            {
                var state = new ModelState();

                if (bindingContext.ModelState.TryGetValue("AcsItemInDetails", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["AcsItemInDetails"] ?? "{}";
                        model.AcsItemInDetails = JsonConvert.DeserializeObject<List<AcsItemInDetailViewModel>>(json, new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local});
                        state.Errors.Clear();
                    }
                }
            }
            return model;

        }
    }
}