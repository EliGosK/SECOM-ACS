using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsVisitor : IAcsRequest
    {
        public string DocumentType => AcsRequestTypeCodes.Visitor;
        public string Requester => this.CreateBy;
        public EmployeeInformation RequestEmployee { get; set; }
        public Misc Purpose { get; set; }

        public TransactionAcs[] ToTransactions(string user)
        {
            var results = new List<TransactionAcs>();
            var startDate = this.EntryDateFrom;
            while (DateTime.Compare(startDate.Date, this.EntryDateTo.Date)<=0)
            {
                foreach (var detail in this.AcsVisitorDetails)
                {
                    results.Add(new TransactionAcs()
                    {
                        TranID = Guid.NewGuid(),
                        ReqNo = this.ReqNo,
                        EntryDateFrom = startDate,
                        EntryTimeFrom = this.EntryTimeFrom,
                        EntryDateTo = startDate,
                        EntryTimeTo = this.EntryTimeTo,
                        DetailID = detail.DetailID,
                        Status = (byte)TransactionStatus.SendCardToACS,
                        UpdateBy = user,
                        UpdateDate = DateTime.Now
                    });
                }
               
                startDate = startDate.AddDays(1);
            }
            return results.ToArray();
        }
    }

    [Flags]
    public enum LoadAcsVisitorOptions
    {
        None = 0,
        RequestEmployee = 1,
        Approval = 2,
        Purpose = 4,
        VisitorDetail = 8,
        RequestAreaMapping = 16,
        Header = RequestEmployee | Purpose | Approval,
        All = RequestEmployee | Approval | Purpose | VisitorDetail | RequestAreaMapping
    }

    [LocalizeProperty("VerifyType", "VerifyTypeEN")]
    [LocalizeProperty("VerifyType", "th", "VerifyTypeTH")]
    public partial class AcsVisitorDetailDataView
    {

    }

    public class OverlapVisitorCardNoException : Exception
    {
        public OverlapVisitorCardNoException(string message, CheckOverlapCardNoList dataItem) 
            : base(message)
        {
            this.DataItem = dataItem;
        }

        public CheckOverlapCardNoList DataItem { get; private set; }
    }

    public class OverlapPeriodVisitorCardNoException : Exception
    {
        public OverlapPeriodVisitorCardNoException(string message, CheckOverlapPeriodCardNoList dataItem)
            : base(message)
        {
            this.DataItem = dataItem;
        }

        public CheckOverlapPeriodCardNoList DataItem { get; private set; }
    }

    [LocalizeProperty("VerifyType", "VerifyTypeEN")]
    [LocalizeProperty("VerifyType", "th", "VerifyTypeTH")]
    public partial class AcsVisitorTransactionDataView
    {

    }
}
