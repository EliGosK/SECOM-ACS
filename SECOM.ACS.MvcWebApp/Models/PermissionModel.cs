using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class PermissionModel
    {
        public PermissionModel()
        {
            this.Items = new List<PermissionModel>();
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("hasChildren")]
        public bool HasChildren { get; set; }
        public IList<PermissionModel> Items { get; set; }

        public bool ShouldSerializeItems()
        {
            return this.Items != null && this.Items.Count > 0;
        }
    }
}