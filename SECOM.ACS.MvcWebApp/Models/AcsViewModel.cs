using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class AcsViewModel
    {
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }

        public string RequestNo { get; set; }

        [Display(Name = "Auth. Date From")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Auth. Date To")]
        public DateTime DateTo { get; set; }

        [Display(Name = "Area")]
        public int AreaID { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Request By")]
        public string RequestUser { get; set; }

        public Area Area { get; set; }
    }
}