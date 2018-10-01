using CSI.Core;
using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAcsRequestInquriyRepository 
    {
        IEnumerable<RequestDataView> GetRequestDataBySearchCriteria(RequestInquriySearchCriteria criteria);
        IEnumerable<RequestDH01DataView> GetDashboardRequestInProgress(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH02DataView> GetDashboardRequestWaitToApprover(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH03DataView> GetDashboardReqWaitToAcknowledge(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH04DataView> GetDashboardSecurityRoom(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH05DataView> GetDashboardItemOut(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH06DataView> GetDashboardItemIn(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH07DataView> GetDashboardLending(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH08DataView> GetDashboardWitness(DashboardSearchCriteria criteria);
        IEnumerable<PrivilegeViewDSHData> GetPermissionDashboard(string rold);
    }
   
}
