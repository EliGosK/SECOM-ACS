using ModelMetadataExtensions;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    [MetadataConventions(ResourceType = typeof(AreaForEmployeeEntryViewModelResource))]
    public class AreaMappingViewModel
    {
        public string ID { get; set; }
        public int AreaID { get; set; }
        public string FactoryCode { get; set; }
        [UIHint("Area")]
        public string AreaName { get; set; }
        public bool IsMainArea { get; set; }

        public string Name {
            get
            {
                return string.Format("{0}: {1}", this.FactoryCode, this.AreaName);
            }
        }
    }
}