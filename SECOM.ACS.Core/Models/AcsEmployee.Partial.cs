using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsEmployee : IAcsRequest
    {
        public string DocumentType => AcsRequestTypeCodes.Employee;
        public string Requester => this.CreateBy;
        public EmployeeInformation RequestEmployee { get; set; }
        public Misc Purpose { get; set; }

        public TransactionAcs[] ToTransactions(string user)
        {
            var results = new List<TransactionAcs>();
            switch (this.RequestFor)
            {
                case RequestFors.Employee:
                    foreach (var detail in this.AcsEmployeeDetails)
                    {
                        results.Add(new TransactionAcs()
                        {
                            TranID = Guid.NewGuid(),
                            ReqNo = this.ReqNo,
                            EntryDateFrom = this.EntryDateFrom,
                            EntryTimeFrom = this.EntryTimeFrom,
                            EntryDateTo = this.EntryDateTo,
                            EntryTimeTo = this.EntryTimeTo,
                            DetailID = detail.DetailID,
                            Status = (byte)TransactionStatus.SendCardToACS,
                            UpdateBy = user,
                            UpdateDate = DateTime.Now
                        });
                    }
                  
                    break;
                case RequestFors.BusinessTrip:
                    var startDate = this.EntryDateFrom;
                    foreach (var detail in this.AcsEmployeeDetails)
                    {
                        results.Add(new TransactionAcs()
                        {
                            TranID = Guid.NewGuid(),
                            ReqNo = this.ReqNo,
                            EntryDateFrom = this.EntryDateFrom,
                            EntryTimeFrom = this.EntryTimeFrom,
                            EntryDateTo = this.EntryDateTo,
                            EntryTimeTo = this.EntryTimeTo,
                            DetailID = detail.DetailID,
                            Status = (byte)TransactionStatus.ActiveWithoutAssignedCardID,
                            UpdateBy = user,
                            UpdateDate = DateTime.Now
                        });
                    }
                    break;
            }

            return results.ToArray();
        }
    }

    [Flags]
    public enum LoadAcsEmployeeOptions
    {
        None = 0,
        RequestEmployee = 1,
        Purpose = 2,
        Approval = 4,
        EmployeeDetail = 8,
        RequestAreaMapping = 16,
        Header = RequestEmployee | Purpose | Approval,
        All = RequestEmployee | Purpose | Approval | EmployeeDetail | RequestAreaMapping
    }

    [LocalizeProperty("EmpName","EmpNameEN")]
    [LocalizeProperty("EmpName","th", "EmpNameTH")]
    [LocalizeProperty("DepartmentName", "DepartmentNameEN")]
    [LocalizeProperty("DepartmentName", "th", "DepartmentNameTH")]
    public partial class AcsEmployeeDetailDataView
    {

    }

    public partial class AcsEmployeeDetail
    {
        public EmployeeInformation Employee { get; set; }
    }
}
