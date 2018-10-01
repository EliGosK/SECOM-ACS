using CSI.ComponentModel;
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
    public partial class AccessControlService 
    {
        public AcsVisitor GetAcsVisitor(string requestNo, LoadAcsVisitorOptions loadOptions)
        {
            using (var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsVisitors.Get(requestNo);
                LoadData(u, dataItem, loadOptions);
                return dataItem;
            }
        }

        private void LoadData(IUnitOfWork unitOfWork, AcsVisitor acs, LoadAcsVisitorOptions loadOptions)
        {
            if (acs == null) { return; }

            if ((loadOptions & LoadAcsVisitorOptions.RequestEmployee) > 0)
            {
                acs.RequestEmployee = unitOfWork.Employees.GetInformation(acs.CreateBy);
            }

            if ((loadOptions & LoadAcsVisitorOptions.VisitorDetail) > 0)
            {
                acs.AcsVisitorDetails = unitOfWork.AcsVisitorDetails.Find(t => t.ReqNo == acs.ReqNo).ToList();
            }

            if ((loadOptions & LoadAcsVisitorOptions.Approval) > 0)
            {
                acs.ReqApproverList = unitOfWork.ReqApprovers.GetLatest(acs.ReqNo)
                    .ToList();
                foreach (var r in acs.ReqApproverList)
                {
                    r.ApproveEmployee = unitOfWork.Employees.GetByUserName(r.ApproveUserName);
                    if (r.AreaID.HasValue)
                    {
                        r.Area = unitOfWork.Areas.Get(r.AreaID.Value);
                    }
                }
            }

            if ((loadOptions & LoadAcsVisitorOptions.RequestAreaMapping) > 0)
            {
                acs.ReqAreaMappings = unitOfWork.ReqAreaMappings.Find(t => t.ReqNo == acs.ReqNo).ToList();
            }

            if ((loadOptions & LoadAcsVisitorOptions.Purpose) > 0)
            {
                acs.Purpose = unitOfWork.Miscs.Find(t => t.MiscID == acs.PurposeCodeID).FirstOrDefault();
            }
        }

        public ObjectResult CreateAcsVisitor(AcsVisitor entity)
        {
            bool acsInserted = false;
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    u.AcsVisitors.Add(entity);
                    acsInserted = true;

                    //Insert Visitor Detail
                    foreach (var v in entity.AcsVisitorDetails)
                    {
                        v.ReqNo = entity.ReqNo;
                        u.AcsVisitorDetails.Add(v);
                    }

                    // Insert Req Approver List
                    foreach (var approver in entity.ReqApproverList)
                    {
                        approver.ReqNo = entity.ReqNo;
                        u.ReqApprovers.Add(approver);
                    }

                    // Insert Request Area Mapping
                    foreach (var area in entity.ReqAreaMappings)
                    {
                        area.ReqNo = entity.ReqNo;
                        u.ReqAreaMappings.Add(area);
                    }
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
                catch (Exception ex)
                {
                    if (acsInserted)
                    {
                        u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                        u.ReqAreaMappings.RemovesByRequestNo(entity.ReqNo);
                        u.AcsVisitorDetails.RemovesByRequestNo(entity.ReqNo);
                        u.AcsVisitors.Remove(entity);
                        u.Complete();
                    }
                    return ObjectResult.Fail(ex);
                }
            }
        }

        public ObjectResult UpdateAcsVisitor(AcsVisitor entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsVisitors.Edit(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {

                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateAcsVisitorForAuthor(AcsVisitor entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsVisitors.Update(entity);
                    u.AcsVisitorDetails.RemovesByRequestNo(entity.ReqNo);
                    foreach (var detail in entity.AcsVisitorDetails)
                    {
                        u.AcsVisitorDetails.Add(detail);
                    }
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {

                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult DeleteAcsVisitor(AcsVisitor entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                    u.ReqAreaMappings.RemovesByRequestNo(entity.ReqNo);
                    u.AcsVisitorDetails.RemovesByRequestNo(entity.ReqNo);
                    u.AcsVisitors.Remove(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }

        public IEnumerable<AcsVisitorDetailDataView> GetAcsVisitorDetailDataViews(string requestNo)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsVisitorDetails.GetDataViews(requestNo).ToList();
            }
        }

        public IEnumerable<AcsVisitorTransactionDataView> GetAcsVisitorTransactionDataViews(string requestNo)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsVisitors.GetTransactionViews(requestNo).ToList();
            }
        }
    }
}
