using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class RoleUserForEmployeeEntryViewModel
    {
        public string ID { get; set; }
        [UIHint("User")]
        public int RoleID { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}