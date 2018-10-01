using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(VisitorCardRegistrationViewModelResource))]
    public class VisitorCardRegistrationViewModel
    {
        public System.Guid TranID { get; set; }
        public System.Guid DetailID { get; set; }
        public string VisitorName { get; set; }
        public string Company { get; set; }
        public string AreaName { get; set; }
        public string VerifyTypeName { get; set; }
        public int VerifyTypeID { get; set; }
        public string VerifyNo { get; set; }
        public string CardNo { get; set; }
        public string CardID { get; set; }
        public DateTime? SendAcsDate { get; set; }
        public DateTime EntryDateFrom { get; set; }
        public DateTime EntryDateTo { get; set; }
        public string OriginalCardID { get; set; }
        public bool AllowEditCardRegis { get; set; }

        public bool AllowEdit
        {
            get
            {
                if (String.IsNullOrEmpty(this.CardNo))
                {
                    return DateTime.Compare(this.EntryDateTo.Date, DateTime.Now.Date) >= 0;
                }

                return DateTime.Compare(this.EntryDateFrom.Date, DateTime.Now.Date) >= 0;
            }

        }
        public bool IsAvailable { get { return String.IsNullOrEmpty(this.CardNo); } }
    }
}