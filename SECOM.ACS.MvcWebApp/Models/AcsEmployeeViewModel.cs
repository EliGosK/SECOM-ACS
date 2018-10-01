using CSI.Text;
using CSI.Web.Mvc;
using CsvHelper.Configuration;
using ModelMetadataExtensions;
using Newtonsoft.Json;
using SECOM.ACS.Models;
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
    [MetadataConventions(ResourceType = typeof(AcsEmployeeViewModelResource))]
    [ModelBinder(typeof(AcsEmployeeViewModelBinder))]
    public class AcsEmployeeViewModel
    {
        [Required]
        public string ReqNo { get; set; }

        public string Status { get; set; } = RequestStatus.Requesting;
        [Required]
        public string RequestFor { get; set; }

        public System.DateTime EntryDateFrom { get; set; }

        public int EntryTimeFrom { get; set; }

        public System.DateTime EntryDateTo { get; set; }

        public int EntryTimeTo { get; set; }
        [Required]
        public int PurposeCodeID { get; set; }

        public string OtherPurpose { get; set; }
        public string Note { get; set; }

        [Required]
        public int[] Areas { get; set; }

        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; } = DateTime.Now;
        public string UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; } = DateTime.Now;

        public MiscViewModel Purpose { get; set; }
      
        // Superior Approval
        public string SuperiorApprovalID { get; set; }
        public string SuperiorApproveUserName { get; set; }
        public string SuperiorApprovalCode { get; set; }
        public DateTime? SuperiorApprovalDate { get; set; }
        public string SuperiorRejectReason { get; set; }
        public EmployeeViewModel SuperiorEmployee { get; set; }
        public string SuperiorReferenceApprovalID { get; set; }

        public EmployeeInformationViewModel RequestEmployee { get; set; }
        public string RequestEmployeeName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.EmployeeName; } }
        public string RequestEmployeeDepartmentName { get { return this.RequestEmployee == null ? "" : this.RequestEmployee.Department; } }

        // Area Approval
        public IList<ReqApproverListViewModel> AreaApprovals { get; set; } = new List<ReqApproverListViewModel>();
        // Employee Detail
        public IList<AcsEmployeeDetailViewModel> AcsEmployeeDetails { get; set; } = new List<AcsEmployeeDetailViewModel>();
        public RequestViewMode ViewMode { get; set; } = RequestViewMode.View;
        public string TempEmployeeDataId { get; set; } = TextGenerator.Generate(32);
        public string TempBusinessDataId { get; set; } = TextGenerator.Generate(32);
        public string ReferenceReqNo { get; set; }

        public bool AllowCancelRequest(IPrincipal user)
        {
            switch (this.Status)
            {
                case RequestStatus.Requesting:
                case RequestStatus.Approving:
                case RequestStatus.Approved:
                    return String.Compare(user.Identity.Name, this.CreateBy, true) == 0 && DateTime.Now.Date < this.EntryDateFrom.Date;
                default:
                    return false;
            }
        }

        public string TempDataId
        {
            get {
                return this.RequestFor == RequestFors.Employee ? this.TempEmployeeDataId : this.TempBusinessDataId;
            }
        }

        public bool AllowAreaApproval()
        {
            if (String.IsNullOrEmpty(this.SuperiorApproveUserName)) {
                return true;
            }

            if (String.IsNullOrEmpty(this.SuperiorReferenceApprovalID) && this.SuperiorApprovalCode == ApprovalCode.Approve)
            {
                return true;
            }

            if (!String.IsNullOrEmpty(this.SuperiorReferenceApprovalID) && this.SuperiorApprovalCode != ApprovalCode.Reject)
            {
                return true;
            }

            return false;
        }
    }

    [MetadataConventions(ResourceType = typeof(AcsEmployeeDetailViewModelResource))]
    public class AcsEmployeeDetailViewModel
    {
        public Guid DetailID { get; set; } = Guid.NewGuid();
        public int Seq { get; set; }
        [Required]
        [StringLength(50)]
        [UIHint("EmployeeID")]
        public string EmployeeID { get; set; }
        [Required]
        [StringLength(256)]
        public string EmployeeName { get; set; }
        [Required]
        [StringLength(256)]
        public string DepartmentName { get; set; }
    }

    public class AcsEmployeeImportData
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
    }

    public sealed class AcsEmployeeImportDataMap : CsvClassMap<AcsEmployeeImportData>
    {
        public AcsEmployeeImportDataMap()
        {
            Map(m => m.EmployeeID).Name("EmpID", "EmployeeID", "Employee ID");
        }
    }
    
    public sealed class AcsBusinessTripImportDataMap : CsvClassMap<AcsEmployeeImportData>
    {
        public AcsBusinessTripImportDataMap()
        {
            Map(m => m.EmployeeName).Name("EmpName", "EmployeeName", "Employee Name");
            Map(m => m.DepartmentName).Name("Department", "Department Name");
        }
    }


    public class ImportDataError
    {
        public string[] Data { get; set; }
        public string Error { get; set; }
    }

    public class AcsEmployeeViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as AcsEmployeeViewModel;
            if (model != null)
            {
                var state = new ModelState();
                if (bindingContext.ModelState.TryGetValue("AreaApprovals", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["AreaApprovals"] ?? "{}";
                        model.AreaApprovals = JsonConvert.DeserializeObject<List<ReqApproverListViewModel>>(json);
                        state.Errors.Clear();
                    }
                }

                if (bindingContext.ModelState.TryGetValue("AcsEmployeeDetails", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["AcsEmployeeDetails"] ?? "{}";
                        model.AcsEmployeeDetails = JsonConvert.DeserializeObject<List<AcsEmployeeDetailViewModel>>(json);
                        state.Errors.Clear();
                    }
                }
            }
            return model;

        }
    }

}