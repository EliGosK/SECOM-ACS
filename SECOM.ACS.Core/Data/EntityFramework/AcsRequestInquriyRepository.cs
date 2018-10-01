using CSI.Core;
using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data.EntityFramework
{
    class AcsRequestInquriyRepository : EntityRepository <ACSContext, RequestDataView, string>, IAcsRequestInquriyRepository
    {
        public AcsRequestInquriyRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<RequestDataView> GetRequestDataBySearchCriteria(RequestInquriySearchCriteria criteria)
        {
            var dataViews = Context.GetRequestDataViewsByCriteria(criteria.ObjectIDValue, criteria.ReqNo, criteria.CreateBy, criteria.AreaValue, criteria.EntryDateFrom, criteria.EntryDateTo, criteria.StatusValue, criteria.AssetCode).ToList();
            return dataViews;
        }

        public IEnumerable<RequestDH01DataView> GetDashboardRequestInProgress(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardRequestInProgress(criteria.User,criteria.DocumentType,criteria.Status)
                .Where(t=>String.Compare(t.ObjectID,criteria.DocumentType)==0 || String.IsNullOrEmpty(criteria.DocumentType))
                .Where(t=>t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                .ToList();
        }

        public IEnumerable<RequestDH02DataView> GetDashboardRequestWaitToApprover(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardRequestWaitToApprover(criteria.User, criteria.DocumentType, criteria.Status)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<RequestDH03DataView> GetDashboardReqWaitToAcknowledge(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardReqWaitToAcknowledge(criteria.User, criteria.DocumentType)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<RequestDH04DataView> GetDashboardSecurityRoom(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardSecurityRoom(criteria.User, criteria.DocumentType)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<RequestDH05DataView> GetDashboardItemOut(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardItemOut(criteria.User)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<RequestDH06DataView> GetDashboardItemIn(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardItemIn(criteria.User)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<RequestDH07DataView> GetDashboardLending(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardLending(criteria.User)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
            
        }

        public IEnumerable<RequestDH08DataView> GetDashboardWitness(DashboardSearchCriteria criteria)
        {
            return Context.GetDashboardWitness(criteria.User)
                    .Where(t => String.Compare(t.ObjectID, criteria.DocumentType) == 0 || String.IsNullOrEmpty(criteria.DocumentType))
                    .Where(t => t.ReqStatus == criteria.Status || String.IsNullOrEmpty(criteria.Status))
                    .ToList();
        }

        public IEnumerable<PrivilegeViewDSHData> GetPermissionDashboard(string rold)
        {
            return Context.GetPrivilegeViewUser(rold).ToList();
        }
    }


}
