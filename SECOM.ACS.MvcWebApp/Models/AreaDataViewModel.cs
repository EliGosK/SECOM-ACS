using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class AreaDataViewModel
    {
        public int AreaID { get; set; }
        public string AreaDisplay { get; set; }
        public string FactoryCode { get; set; }

        public string Name {
            get {
                return String.Format("{0} : {1}", this.FactoryCode, this.AreaDisplay);
            }
        }
    }
}