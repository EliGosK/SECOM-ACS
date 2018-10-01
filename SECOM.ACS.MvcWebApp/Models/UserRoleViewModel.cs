
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class UserRoleViewModel
    {
       
        public string UserName { get; set; }
        public string[] Roles { get; set; }
    }
}