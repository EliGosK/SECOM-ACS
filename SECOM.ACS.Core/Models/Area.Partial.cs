using CSI.ComponentModel;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("AreaDisplay", "AreaDisplayEN")]
    [LocalizeProperty("AreaDisplay","th","AreaDisplayTH")]
    public partial class Area
    {
        public bool ShouldSerializeArea()
        {
            return false;
        }
    }

    public class  AreaSearchCriteria : ISearchCriteria
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public string[] FactoryCode { get; set; }
        public string[] Status { get; set; }

        public void EnsureDataValid()
        {
            
        }

        public string StatusValue
        {
            get {
                if (this.Status != null && this.Status.Length > 0)
                {
                    return String.Join(",", this.Status);
                }
                return null;
            }
        }

        public string FactoryCodeString
        {
            get
            {
                if (this.FactoryCode == null || this.FactoryCode.Length == 0) { return null; }
                return String.Join(",", this.FactoryCode);
            }
        }
    }

    public enum AreaLoadOptions
    {
        None = 0,
        Gate = 1,
        All = Gate
    }

    [LocalizeProperty("AreaDisplay", "AreaDisplayEN")]
    [LocalizeProperty("AreaDisplay", "th", "AreaDisplayTH")]
    public partial class AreaDataView
    {

    }

    
}
