using ModelMetadataExtensions;
using SECOM.ACS.Identity;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(RoleViewModelResource))]
    public class RoleViewModel
    {
        public int RoleID { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Description { get; set; }

        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }
    }

    public class SearchRoleCriteria
    {
        public string Name { get; set; }
    }
}