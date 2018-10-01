using CSI.Text;
using CSI.Web.Mvc;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ModelMetadataExtensions;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AcsVisitorViewModelResource))]
    [ModelBinder(typeof(AcsVisitorViewModelBinder))]
    public class AcsVisitorViewModel
    {
        [Required]
        public string ReqNo { get; set; }

        public string Status { get; set; } = RequestStatus.Requesting;

        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }


        public DateTime EntryDateFrom { get; set; }

        public DateTime EntryDateTo { get; set; }

        public int EntryTimeFrom { get; set; }

        public int EntryTimeTo { get; set; }

        [Required]
        public int[] Areas { get; set; }

        [Required]
        public string PurposeCodeID { get; set; }
            
        public string OtherPurpose { get; set; }

        public string Note { get; set; }

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

        public IList<AcsVisitorDetailViewModel> AcsVisitorDetails { get; set; } = new List<AcsVisitorDetailViewModel>();
        public MiscViewModel Purpose { get; set; }

        public RequestViewMode ViewMode { get; set; } = RequestViewMode.View;
        public string ReferenceReqNo { get; set; }
        public string TempDataId { get; set; } = TextGenerator.Generate(32);
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

        public bool AllowAreaApproval()
        {
            if (String.IsNullOrEmpty(this.SuperiorApproveUserName))
            {
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

    [MetadataConventions(ResourceType = typeof(AcsVisitorDetailViewModelResource))]
    public class AcsVisitorDetailViewModel
    {
        public Guid DetailID { get; set; } = Guid.NewGuid();

        public int Seq { get; set; }

        public int VerifyTypeID { get; set; }

        [MaxLength(50)]
        public string VerifyNo { get; set; }

        [Required]
        [MaxLength(256)]
        public string VisitorName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Company { get; set; }

        [MaxLength(256)]
        public string DepartmentName { get; set; }

        [MaxLength(256)]
        public string ItemInOut { get; set; }

        [MaxLength(256)]
        public string Photographing { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        public string VerifyType { get; set; }
    }

    public class AcsVisitorViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as AcsVisitorViewModel;
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

                if (bindingContext.ModelState.TryGetValue("AcsVisitorDetails", out state))
                {
                    if (state.Errors.Count > 0)
                    {
                        var json = controllerContext.HttpContext.Request["AcsVisitorDetails"] ?? "{}";
                        model.AcsVisitorDetails = JsonConvert.DeserializeObject<List<AcsVisitorDetailViewModel>>(json);
                        state.Errors.Clear();
                    }
                }
            }
            return model;

        }
    }

    //public class VisitorTransactionViewModel
    //{
    //    public string TransactionId { get; set; }

    //    public DateTime AuthDate { get; set; }

    //    public DateTime AuthTimeFrom { get; set; }

    //    public DateTime AuthTimeTo { get; set; }

    //    public string VerifyType { get; set; }

    //    public string VerifyNo { get; set; }

    //    public string ConfirmVerifyNo { get; set; }

    //    public string CardNo { get; set; }

    //    public string Name { get; set; }

    //    public string ItemInOut { get; set; }
        
    //    public string Photographing { get; set; }
    //}

    public class AcsVisitorImportData
    {
        public int VerifyTypeID { get; set; }
        public string VerifyType { get; set; }
        public string VerifyNo { get; set; }
        public string VisitorName { get; set; }
        public string Department { get; set; }

        public string Company { get; set; }
        public string ItemInOut { get; set; }
        public string Photographing { get; set; }
        public string Description { get; set; }
    }

    public sealed class AcsVisitorImportDataMap : CsvClassMap<AcsVisitorImportData>
    {
        public AcsVisitorImportDataMap()
        {
            Map(m => m.VerifyTypeID).Name("Verify Type").TypeConverter(new VisitorVerifyTypeConverter());
            Map(m => m.VerifyNo).Name("Verify No", "Verify No.");
            Map(m => m.VisitorName).Name("Name", "Visitor Name");
            Map(m => m.Company).Required("Company");
            Map(m => m.Department).Name("Department");
            Map(m => m.ItemInOut).Name("ItemInOut", "Item In Out");
            Map(m => m.Photographing).Name("Photographing");
            Map(m => m.Description).Name("Description");
        }
    }

    public class VisitorVerifyTypeConverter : ITypeConverter
    {
        public bool CanConvertFrom(Type type)
        {
            return true;
        }

        public bool CanConvertTo(Type type)
        {
            return true;
        }

        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            var values = GetList();
            var findItem = values.Where(t => String.Compare(t.Key, text, true) == 0).Select(t => t.Value).ToList();
            return findItem.Count==0 ? 0 : findItem[0];
        }

        public string ConvertToString(TypeConverterOptions options, object value)
        {
            var values = GetList();
            if (values.ContainsValue((int)value))
            {
                return values.FirstOrDefault(t => t.Value == (int)value).Key;
            }
            return "N/A";
        }

        private Dictionary<string, int> GetList()
        {
            var values = new Dictionary<string, int>();
            values.Add("ID Card", VisitorVerifyTypes.IDCard);
            values.Add("Passport", VisitorVerifyTypes.PassportNo);
            return values;
        }
    }

    [MetadataConventions(ResourceType = typeof(AcsVisitorTransactionDataViewModelResource))]
    public class AcsVisitorTransactionDataViewModel
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public System.DateTime EntryDateFrom { get; set; }
        public System.TimeSpan EntryTimeFrom { get; set; }
        public System.DateTime EntryDateTo { get; set; }
        public System.TimeSpan EntryTimeTo { get; set; }
        public Nullable<int> VerifyTypeID { get; set; }
        public string VerifyTypeName { get; set; }
        public string VerifyNo { get; set; }
        public string CardID { get; set; }
        public string CardNo { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string DepartmentName { get; set; }
    }
}