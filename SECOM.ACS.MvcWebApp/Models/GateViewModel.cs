using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class GateViewModel
    {
        public string GateID { get; set; }
        public string FactoryCode { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}