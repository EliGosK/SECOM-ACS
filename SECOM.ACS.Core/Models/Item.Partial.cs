
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("ItemDisplay", "ItemDisplayEN")]
    [LocalizeProperty("ItemDisplay", "th", "ItemDisplayTH")]
    public partial class Item
    {
        public Misc ItemType { get; set; }
    }

    [LocalizeProperty("ItemDisplay", "ItemDisplayEN")]
    [LocalizeProperty("ItemDisplay", "th", "ItemDisplayTH")]
    [LocalizeProperty("MiscDisplay", "MiscDisplayEN")]
    [LocalizeProperty("MiscDisplay", "th", "MiscDisplayTH")]
    public partial class ItemDataView
    {

    }

    public class ItemSearchCriteria
    {
        public string ItemName { get; set; }
        public string[] RequestType { get; set; }
        public string ItemType { get; set; }
        public string[] Status { get; set; }

        public bool IsItemIn { get; set; }
        public bool IsItemOut { get; set; }
        public bool IsPhoto { get; set; }

        public string StatusValue
        {
            get
            {
                if (this.Status != null && this.Status.Length > 0)
                {
                    return String.Join(",", this.Status);
                }
                return null;
            }
        }

        public void EnsureDataValid()
        {

            if (RequestType != null && this.RequestType.Length > 0)
            {
                IsItemOut = this.RequestType.Contains(AcsRequestTypes.ItemOut);
                IsPhoto = this.RequestType.Contains(AcsRequestTypes.Photographing);
                IsItemIn = this.RequestType.Contains(AcsRequestTypes.ItemIn);
            }
        }
    }

    public class ItemInUseException : Exception
    {
        public ItemInUseException() : base("Data is in use")
        {

        }
    }

   
}
