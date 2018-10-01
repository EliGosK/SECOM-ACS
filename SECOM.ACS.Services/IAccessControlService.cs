using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public interface IAccessControlService : IMasterService
    {
        ObjectResult UpdateAcsRequest(IAcsRequest request);

        AcsEmployee GetAcsEmployee(string requestNo, LoadAcsEmployeeOptions loadOptions);
        ObjectResult CreateAcsEmployee(AcsEmployee entity);
        ObjectResult UpdateAcsEmployee(AcsEmployee entity);
        ObjectResult UpdateAcsEmployeeForAuthor(AcsEmployee entity);
        ObjectResult DeleteAcsEmployee(AcsEmployee entity);
        IEnumerable<AcsEmployeeDetailDataView> GetAcsEmployeeDetailDataViews(string requestNo);

        AcsVisitor GetAcsVisitor(string requestNo, LoadAcsVisitorOptions loadOptions);
        ObjectResult CreateAcsVisitor(AcsVisitor entity);
        ObjectResult UpdateAcsVisitor(AcsVisitor entity);
        ObjectResult UpdateAcsVisitorForAuthor(AcsVisitor entity);
        ObjectResult DeleteAcsVisitor(AcsVisitor entity);
        IEnumerable<AcsVisitorDetailDataView> GetAcsVisitorDetailDataViews(string requestNo);
        IEnumerable<AcsVisitorTransactionDataView> GetAcsVisitorTransactionDataViews(string requestNo);

        //AcsVIP GetAcsVIP(string requestNo, LoadAcsVIPOptions loadOptions);
        IEnumerable<VIPCardRegistrationView> GetVIPCardRegistrationViews(VIPCardRegistrationSearchCriteria criteria);
        ObjectResult CreateAcsVIP(AcsVIP entity);
        ObjectResult UpdateAcsVIP(AcsVIP entity);
        ObjectResult DeleteAcsVIP(AcsVIP entity);

        AcsItemIn GetAcsItemIn(string requestNo, LoadAcsItemInOptions loadOptions);
        IEnumerable<AcsItemInDetailDataView> GetAcsItemInDetailDataViews(string reqNo);
        ObjectResult CreateAcsItemIn(AcsItemIn entity);
        ObjectResult UpdateAcsItemIn(AcsItemIn entity);
        ObjectResult UpdateAcsItemInDetail(List<AcsItemInDetail> datas);
        ObjectResult UpdateAcsItemInForAuthor(AcsItemIn entity);
        ObjectResult DeleteAcsItemIn(AcsItemIn entity);

        AcsItemOut GetAcsItemOut(string requestNo, LoadAcsItemOutOptions loadOptions);
        ObjectResult CreateAcsItemOut(AcsItemOut entity);
        ObjectResult UpdateAcsItemOut(AcsItemOut entity);
        ObjectResult DeleteAcsItemOut(AcsItemOut entity);
        ObjectResult UpdateAcsItemOutForAuthor(AcsItemOut entity);
        IEnumerable<AcsItemOutDetailDataView> GetAcsItemOutDetailDataViews(string requestNo);

        AcsPhoto GetAcsPhoto(string requestNo, LoadAcsPhotoOptions loadOptions);
        ObjectResult CreateAcsPhoto(AcsPhoto entity);
        ObjectResult UpdateAcsPhoto(AcsPhoto entity);
        ObjectResult DeleteAcsPhoto(AcsPhoto entity);

        ReqApproverList GetReqApproverList(Guid approvalID);
        IEnumerable<ReqApproverList> GetReqApproverListByRequestNo(string requestNo);
        IEnumerable<ReqApproverList> GetLatestReqApproverList(string requestNo);
        ObjectResult CreateReqApproverList(ReqApproverList entity);
        ObjectResult UpdateReqApproverList(ReqApproverList entity);

        IEnumerable<EmployeeApprovalDataView> GetSuperiorApprover(string user);
        IEnumerable<EmployeeApprovalDataView> GetAreaApprover(int areaId);
        IEnumerable<EmployeeApprovalDataView> GetAcknowledgeApprover(string name);
        IEnumerable<EmployeeApprovalDataView> GetGAApprover();

        IEnumerable<RequestDataView> GetRequestDataBySearchCriteria(RequestInquriySearchCriteria criteria);

        IEnumerable<RequestDH01DataView> GetDashboardForRequestInProgress(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH02DataView> GetDashboardForRequestWaitToApprover(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH03DataView> GetDashboardForRequestWaitToAcknowledge(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH04DataView> GetDashboardForSecurityRoom(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH05DataView> GetDashboardForItemOut(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH06DataView> GetDashboardForItemIn(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH07DataView> GetDashboardForLending(DashboardSearchCriteria criteria);
        IEnumerable<RequestDH08DataView> GetDashboardForWitness(DashboardSearchCriteria criteria);
        IEnumerable<PrivilegeViewDSHData> GetPermissionDashboard(string username);

        TransactionAcs GetAcsTransaction(Guid transactionId);
        IEnumerable<TransactionAcs> GetAcsTransactionsByRequestNo(string requestNo);
        ObjectResult CreateAcsTransactions(params TransactionAcs[] entities);
        ObjectResult UpdateAcsTransactions(params TransactionAcs[] entities);

        ObjectResult UpdateVisitorCard(ReceiveReturnVisitorCardDataView dataView);
        IEnumerable<ReceiveReturnVisitorCardDataView> GetReceiveReturnVisitorCardDataViews(VisitorCardDataSearchCriteria criteria);
        ObjectResult ReturnVisitorCard(ReceiveReturnVisitorCardDataView entity);
        ObjectResult ReceiveVisitorCard(ReceiveReturnVisitorCardDataView entity);

        IEnumerable<ReceiveReturnBusinessTripCardDataView> GetReceiveReturnBusinessTripCardDataViews(BusinessTripCardDataSearchCriteria criteria);
        ObjectResult ReceiveBusinessCard(ReceiveReturnBusinessTripCardDataView entity);
        ObjectResult ReturnBusinessCard(ReceiveReturnBusinessTripCardDataView entity);

        ObjectResults<CheckOverlapPeriodCardNoList> CheckOverlapCardRegistration(BusinessTripCardRegistrationView dataView);
    }
}
