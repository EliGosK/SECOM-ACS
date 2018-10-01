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
    [MetadataConventions(ResourceType = typeof(VIPCardRegistrationViewModelResource))]
    public partial class VIPCardRegistrationViewModel
    {
        public string CardNo { get; set; }
        public string CardID { get; set; }
        public string RequestNo { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        [Required]
        public string Name { get; set; }
        public string Company { get; set; }
        public Nullable<System.DateTime> EntryDateFrom { get; set; }
        public Nullable<System.DateTime> EntryDateTo { get; set; }
        public string Description { get; set; }
        [Required]
        public int PositionID { get; set; }
        public string PositionName { get; set; }
        [Required]
        public int[] SelectedAreaList { get; set; }       
        public AreaDataViewModel[] Area { get; set; }

        //public int[] DefaultArea { get; set; }
    }
}