using ModelMetadataExtensions;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Resources;
using SECOM.ACS.MvcWebApp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AcsPhotoViewModelResource))]
    public class AcsPhotoViewModel
    {
        [Required]
        public string ReqNo { get; set; }

        public string Status { get; set; } = RequestStatus.Requesting;

        public DateTime TakePhotoDateFrom { get; set; }

        public int TakePhotoTimeFrom { get; set; }

        public DateTime TakePhotoDateTo { get; set; }

        public int TakePhotoTimeTo { get; set; }

        public int AreaID { get; set; }

        [Required]
        public string PhotoByType { get; set; }

        public string PhotoEmpID { get; set; }

        public string TakePhotoName { get; set; }
                
        public string WitnessEmpID { get; set; }

        [Required]
        public string TargetItem { get; set; }

        public int EquipItemID { get; set; }

        public string OtherEquip { get; set; }

        public int PurposeCodeID { get; set; }

        public string OtherPurpose { get; set; }
        public bool IsLending { get; set; }
        public string AssetCode { get; set; }
        public DateTime? ActReturnDate { get; set; }
        public string Note { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }

        public string ForceDoneBy { get; set; }

        public Nullable<System.DateTime> ForceDoneDate { get; set; }

        // Superior Approval
        public string SuperiorApprovalID { get; set; }
        public string SuperiorApproveUserName { get; set; }
        public string SuperiorApprovalCode { get; set; }
        public DateTime? SuperiorApprovalDate { get; set; }
        public string SuperiorRejectReason { get; set; }
        public EmployeeViewModel SuperiorApprovalEmployee { get; set; }
        
        // Area Approval        
        public string AreaApprovalID { get; set; }
        [Required]
        public string AreaApproveUserName { get; set; }
        public string AreaApprovalCode { get; set; }
        public DateTime? AreaApprovalDate { get; set; }
        public string AreaRejectReason { get; set; }
        public EmployeeViewModel AreaApprovalEmployee { get; set; }

        // Acknowledge Approval
        public string AcknowledgeApprovalID { get; set; }
        [Required]
        public string AcknowledgeUserName { get; set; }
        public DateTime? AcknowledgeDate { get; set; }
        public EmployeeViewModel AcknowledgeEmployee { get; set; }

        public RequestViewMode ViewMode { get; set; } = RequestViewMode.View;

        public AreaViewModel Area { get; set; }
        public MiscViewModel Purpose { get; set; }
        public ItemViewModel EquipItem { get; set; }
        public EmployeeViewModel PhotoEmployee { get; set; }
        public EmployeeViewModel WitnessEmployee { get; set; }
        public EmployeeViewModel ForceDoneEmployee { get; set; }
        public EmployeeInformationViewModel RequestEmployee { get; set; }

        public string RequestEmployeeName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.EmployeeName; } }
        public string RequestEmployeeDepartmentName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.Department; } }


        public bool IsShowGAToReturnSection(IPrincipal user)
        {
            var isLending = user.Identity.GetUserData().IsLending;
            return String.IsNullOrEmpty(this.AssetCode) ? isLending && this.Status == RequestStatus.Approved : true;
        }

        public bool AllowInputGAToReturn(IPrincipal user)
        {
            return user.Identity.GetUserData().IsLending && !this.ActReturnDate.HasValue;
        }

        public bool AllowCancelRequest(IPrincipal user)
        {
            switch (this.Status)
            {
                case RequestStatus.Requesting:
                case RequestStatus.Approving:
                case RequestStatus.Approved:
                    return String.Compare(user.Identity.Name, this.CreateBy, true) == 0 && DateTime.Now.Date < this.TakePhotoDateFrom.Date;
                default:
                    return false;
            }
        }

        public bool AllowForceDone(IPrincipal user)
        {
            return this.Status == RequestStatus.Approved && DateTime.Now.Date > this.TakePhotoDateFrom.Date && user.Identity.GetUserData().IsVerifyItemIn;
        }
    }
}