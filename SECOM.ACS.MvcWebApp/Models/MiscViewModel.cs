using CSI.Localization;
using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(MiscViewModelResource))]
    [LocalizeProperty("MiscDisplay", "MiscDisplayEN")]
    [LocalizeProperty("MiscDisplay", "th", "MiscDisplayTH")]
    public class MiscViewModel
    {
        public int MiscID { get; set; }

        [Required]
        [StringLength(50)]
        public string MiscType { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string MiscName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string MiscDisplayEN { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string MiscDisplayTH { get; set; }

        [StringLength(256)]
        public string MiscRemark { get; set; }

        public string Status { get; set; }
        public bool DeleteAble { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public object ModelLocalizeManager { get; internal set; }

        public string MiscDisplay { get; set; }
    }
}