using CSI.Localization;
using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(BusinessTripCardRegistrationViewModelResource))]
    public partial class BusinessTripCardRegistrationViewModel
    {
        public Guid TranID { get; set; }
        public Guid DetailID { get; set; }
        [UIHint("BusinessTripCardEditor")]
        public string CardID { get; set; }
        [Required]
        public string CardNo { get; set; }
        public string OriginalCardID { get; set; }
        public string BusinessEmployeeName { get; set; }
        public string EmployeeName { get; set; }
        public string AreaName { get; set; }
        public DateTime EntryDateFrom { get; set; }
        public DateTime EntryDateTo { get; set; }
        public DateTime? SendAcsDate { get; set; }
        public bool AllowEditCardRegis { get; set; }

        public bool AllowEdit {
            get {
                if (this.IsAvailable) {
                    return DateTime.Compare(this.EntryDateTo.Date, DateTime.Now.Date) >= 0;
                }

                return DateTime.Compare(this.EntryDateFrom.Date, DateTime.Now.Date) >= 0;
            }

        }
        //public bool IsAvailable { get { return String.IsNullOrEmpty(this.CardNo); } }
        public bool IsAvailable { get; set; }
        public bool FlagFilter { get { return String.IsNullOrEmpty(this.CardNo); } }
    }
}