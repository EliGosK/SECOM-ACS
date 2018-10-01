using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class VIPCardViewModel
    {
        [Required]
        [StringLength(50)]
        public string CardID { get; set; }

        [Required]
        [StringLength(50)]
        public string CardNo { get; set; }

        public bool IsActive { get; set; }

        public string VistorName { get; set; }

        public string Company { get; set; }

        public string Position { get; set; }

        public TimeSpan Entrance { get; set; }

        public IList<AreaDataViewModel> area { get; set; }

    }
}