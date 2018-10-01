using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("SysMisc","SysMiscValue1")]
    [LocalizeProperty("SysMisc","th", "SysMiscValue2")]
    public partial class SystemMisc
    {
       
    }

    public class SystemMiscKey
    {
        public string SysMiscType { get; set; }
        public string SysMiscCode { get; set; }
    }
}
