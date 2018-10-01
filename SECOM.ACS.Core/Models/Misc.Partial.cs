using CSI.ComponentModel;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    [LocalizeProperty("MiscDisplay", "MiscDisplayEN")]
    [LocalizeProperty("MiscDisplay", "th","MiscDisplayTH" )]
    public partial class Misc
    {

    }

    public class MiscSearchCriteria
    {
        public string Type { get; set; }
        public string Status { get; set; } = "2";

        public void EnsureDataValid()
        {

        }

        public int ActiveStatus {
            get {
                int result;
                if (Int32.TryParse(this.Status, out result))
                {
                    return result;
                }
                return 2;
            }
        }
    }

    public class MiscInUseException : Exception
    {
        public MiscInUseException() : base("Data is in use")
        {

        }
    }

   
}
