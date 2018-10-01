using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Data;
using SECOM.ACS.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public partial class AccessControlService : IAccessControlService
    {
        protected IUnitOfWork CreateUnitOfWork()
        {
            var context = new ACSContext();
            context.Configuration.ProxyCreationEnabled = false;
            return new UnitOfWork(context);
        }

        public ReqApproverList GetReqApproverList(Guid approvalID)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.ReqApprovers.Find(t => t.ApprovalID == approvalID).FirstOrDefault();
            }
        }

        public IEnumerable<ReqApproverList> GetReqApproverListByRequestNo(string requestNo)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.ReqApprovers.Find(t => String.Compare(t.ReqNo,requestNo,true)==0).ToList();
            }
        }

        public IEnumerable<ReqApproverList> GetLatestReqApproverList(string requestNo)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.ReqApprovers.GetLatest(requestNo).ToList();
            }
        }

        public ObjectResult CreateReqApproverList(ReqApproverList entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    unitOfWork.ReqApprovers.Add(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateReqApproverList(ReqApproverList entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    entity.UpdateBy = entity.ApproveUserName;
                    unitOfWork.ReqApprovers.Edit(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }           
        }

        public ObjectResult UpdateAcsRequest(IAcsRequest request)
        {
            var prefix = request.ReqNo.Substring(0, 1);
            switch (prefix)
            {
                case AcsRequestPrefixCharacters.Employee:
                    return this.UpdateAcsEmployeeForAuthor(request as AcsEmployee);
                case AcsRequestPrefixCharacters.Visitor:
                    return this.UpdateAcsVisitor(request as AcsVisitor);
                case AcsRequestPrefixCharacters.ItemIn:
                    return this.UpdateAcsItemIn(request as AcsItemIn);
                case AcsRequestPrefixCharacters.ItemOut:
                    return this.UpdateAcsItemOut(request as AcsItemOut);
                case AcsRequestPrefixCharacters.Photographing:
                    return this.UpdateAcsPhoto(request as AcsPhoto);
                default:
                    return ObjectResult.Fail("Update Acs request fail. Acs request data not found.");
            }
        }


        public IEnumerable<EmployeeApprovalDataView> GetSuperiorApprover(string user)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetSuperiorApprover(user);
            }
        }

        public IEnumerable<EmployeeApprovalDataView> GetAreaApprover(int areaId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetAreaApprover(areaId);
            }
        }

        public IEnumerable<EmployeeApprovalDataView> GetAcknowledgeApprover(string name)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetAcknowledgePerson();
            }
        }

        public IEnumerable<EmployeeApprovalDataView> GetGAApprover()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetGAApprover();
            }
        }

        public IEnumerable<RequestDataView> GetRequestDataBySearchCriteria(RequestInquriySearchCriteria criteria)
        {
            criteria.EnsureDataValid();
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetRequestDataBySearchCriteria(criteria).ToList();
            }       
        }
        public IEnumerable<RequestDH01DataView> GetDashboardForRequestInProgress(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardRequestInProgress(criteria);
            }
        }
        public IEnumerable<RequestDH02DataView> GetDashboardForRequestWaitToApprover(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardRequestWaitToApprover(criteria);
            }
        }
        public IEnumerable<RequestDH03DataView> GetDashboardForRequestWaitToAcknowledge(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardReqWaitToAcknowledge(criteria);
            }
        }
        public IEnumerable<RequestDH04DataView> GetDashboardForSecurityRoom(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardSecurityRoom(criteria);
            }
        }
        public IEnumerable<RequestDH05DataView> GetDashboardForItemOut(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardItemOut(criteria);
            }
        }
        public IEnumerable<RequestDH06DataView> GetDashboardForItemIn(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardItemIn(criteria);
            }
        }
        public IEnumerable<RequestDH07DataView> GetDashboardForLending(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardLending(criteria);
            }
        }
        public IEnumerable<RequestDH08DataView> GetDashboardForWitness(DashboardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetDashboardWitness(criteria);
            }
        }

        public IEnumerable<PrivilegeViewDSHData> GetPermissionDashboard(string username)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.RequestInquiry.GetPermissionDashboard(username).ToList();
            }
        }

       
    }
}
