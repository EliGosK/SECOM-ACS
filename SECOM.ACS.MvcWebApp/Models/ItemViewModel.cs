using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class ItemDataViewModel
    {
        public int ItemID { get; set; }
        public int ItemTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemName { get; set; }
        [Required]
        [StringLength(256)]
        public string ItemDisplayEN { get; set; }
        [Required]
        [StringLength(256)]
        public string ItemDisplayTH { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        public bool IsConfdt { get; set; }
        public bool IsItemOut { get; set; }
        public bool IsItemIn { get; set; }
        public bool IsPhoto { get; set; }
        /// <summary>
        /// IsActive
        /// </summary>
        public string Status { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }

        public string MiscName { get; set; }
        public string MiscDisplay { get; set; }

        public string RequestType
        {
            get
            {
                var values = new string[] { IsItemOut ? "O" : "", IsItemIn ? "I" : "", IsPhoto ? "P" : "" };
                return string.Join(",", values).Trim(',');
            }
        }
    }

    public class ItemViewModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemDisplay { get; set; }
        public int ItemTypeID { get; set; }
    }
 

}