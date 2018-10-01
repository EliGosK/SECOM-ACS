using CSI.Core;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{

    [LocalizeProperty("DocumentType", "DocumentTypeEN")]
    [LocalizeProperty("DocumentType", "th", "DocumentTypeTH")]
    [LocalizeProperty("ReqStatus", "ReqStatusEN")]
    [LocalizeProperty("ReqStatus", "th", "ReqStatusTH")]
    [LocalizeProperty("RequestBy", "RequestByEN")]
    [LocalizeProperty("RequestBy", "th", "RequestByTH")]
    [LocalizeProperty("Area", "AreaDisplayEN")]
    [LocalizeProperty("Area", "th", "AreaDisplayTH")]
    public partial class RequestDataView 
    {
   


    }
    public class RequestInquriySearchCriteria : PagingOptions
    {
        public string[] ObjectID { get; set; }
        public string ReqNo { get; set; }
        public string CreateBy { get; set; }
        public string[] Area { get; set; }
        public DateTime EntryDateFrom { get; set; }
        public DateTime EntryDateTo { get; set; }
        public string[] Status { get; set; }
        public string AssetCode { get; set; }
        public RequsetFilterTypes Filter { get; set; } = RequsetFilterTypes.All;
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

        public string ObjectIDValue
        {
            get
            {
                if (this.ObjectID != null && this.ObjectID.Length > 0)
                {
                    return String.Join(",", this.ObjectID);
                }
                return null;
            }
        }

        public string AreaValue
        {
            get
            {
                if (this.Area != null && this.Area.Length > 0)
                {
                    return String.Join(",", this.Area);
                }
                return null;
            }
        }

        public void EnsureDataValid()
        {

        }
    }
    [Flags]
    public enum RequsetFilterTypes : int
    {
        ReqNo = 1,
        CreateBy = 2,
        All = ReqNo | CreateBy
    }
}
