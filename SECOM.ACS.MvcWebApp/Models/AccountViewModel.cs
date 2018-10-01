using CSI.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class AccountViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Required(AllowEmptyStrings =true)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; }
        [Required]
        public IList<string> Roles { get; set; }
        
        public DateTime RegisteredDateTime { get; set; }
        
        public DateTime LastSignedInDateTime { get; set; }

        public string EmployeeID { get; set; }
        public string FactoryCode { get; set; }
    }

    public class ApplicationUserSearchCriteria : PagingOptions
    {
        public string Username { get; set; }
        public int[] Roles { get; set; }
    }
}