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
        public AcsItemIn GetAcsItemIn(string requestNo,LoadAcsItemInOptions loadOptions)
        {
            using (var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsItemIns.Get(requestNo);
                LoadData(u, dataItem, loadOptions);        
                return dataItem;
            }
        }


        public IEnumerable<AcsItemInDetailDataView> GetAcsItemInDetailDataViews(string requestNo)
        {
            using(var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsItemInDetails.GetDataViews(requestNo).ToList();
                return dataItem;
            }
        }

        private void LoadData(IUnitOfWork unitOfWork, AcsItemIn acs, LoadAcsItemInOptions loadOptions)
        {
            if (acs == null) { return; }

            if ((loadOptions & LoadAcsItemInOptions.RequestEmployee) > 0)
            {
                acs.RequestEmployee = unitOfWork.Employees.GetInformation(acs.CreateBy);
            }

            if ((loadOptions & LoadAcsItemInOptions.Approval) > 0)
            {
                acs.ReqApproverList = unitOfWork.ReqApprovers.Find(t => String.Compare(t.ReqNo, acs.ReqNo, true) == 0)
                 .OrderBy(t => t.Step)
                 .ToList();
                // Superior
                var step1 = acs.ReqApproverList.FirstOrDefault(t => t.Step == 1);
                if (step1 != null)
                {
                    acs.SuperiorApprovalEmployee = unitOfWork.Employees.GetByUserName(step1.ApproveUserName);
                }
                // Area
                var step2 = acs.ReqApproverList.FirstOrDefault(t => t.Step == 2);
                if (step2 != null)
                {
                    acs.AreaApprovalEmployee = unitOfWork.Employees.GetByUserName(step2.ApproveUserName);
                }
                // GA
                acs.AcknowledgeEmployee = unitOfWork.Employees.GetByUserName(acs.AckBy);
            }

            if ((loadOptions & LoadAcsItemInOptions.ItemInDetail) > 0)
            {
                acs.AcsItemInDetails = unitOfWork.AcsItemInDetails.Find(t => String.Compare( t.ReqNo, acs.ReqNo,true)==0)
                    .OrderBy(t => t.Seq)
                    .ToList();
                foreach (var itemDetail in acs.AcsItemInDetails)
                {
                    itemDetail.Item = unitOfWork.Items.Get(itemDetail.ItemID);
                    if (itemDetail.Item != null)
                    {
                        itemDetail.Item.ItemType = unitOfWork.Miscs.Get(itemDetail.Item.ItemTypeID);
                    }
                }
            }
            acs.Purpose = unitOfWork.Miscs.Get(acs.PurposeCodeID);
            acs.Area = unitOfWork.Areas.Get(acs.AreaID);

            if (!String.IsNullOrEmpty(acs.ForceDoneBy))
            {
                acs.ForceDoneEmployee = unitOfWork.Employees.GetByUserName(acs.ForceDoneBy);
            }
        }

        public ObjectResult UpdateAcsItemInForAuthor(AcsItemIn entity)
        {
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsItemIns.Edit(entity);
                    u.AcsItemInDetails.RemovesByRequestNo(entity.ReqNo);
                    foreach (var item in entity.AcsItemInDetails)
                    {
                        item.ReqNo = entity.ReqNo;
                        item.UpdateBy = entity.UpdateBy;
                        u.AcsItemInDetails.Add(item);
                    }
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
                catch (Exception ex)
                {
                    return ObjectResult.Fail(ex);
                }
            }
        }

        /// <summary>
        /// Update AcsItemIn data (Excep AcsItemInDetails)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ObjectResult UpdateAcsItemIn(AcsItemIn entity)
        {
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsItemIns.Edit(entity);
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
                catch(Exception ex)
                {
                    return ObjectResult.Fail(ex);
                }
            }
        }

        public ObjectResult CreateAcsItemIn(AcsItemIn entity)
        {
            bool acsInserted = false;
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    u.AcsItemIns.Add(entity);
                    acsInserted = true;
                    foreach (var item in entity.AcsItemInDetails)
                    {
                        item.ReqNo = entity.ReqNo;
                        item.UpdateBy = entity.UpdateBy;
                        item.UpdateDate = DateTime.Now;
                        u.AcsItemInDetails.Add(item);
                    }
                    foreach (var approver in entity.ReqApproverList)
                    {
                        approver.ReqNo = entity.ReqNo;
                        approver.CreateDate = DateTime.Now;
                        approver.UpdateDate = DateTime.Now;
                        u.ReqApprovers.Add(approver);
                    }
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
                catch (Exception ex)
                {
                    if (acsInserted)
                    {
                        u.AcsItemIns.Remove(entity);
                        u.AcsItemInDetails.RemovesByRequestNo(entity.ReqNo);
                        u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                        u.Complete();
                    }
                    return ObjectResult.Fail(ex);
                }
            }
        }
        public ObjectResult UpdateAcsItemInDetail(List<AcsItemInDetail> datas)
        {
            try
            {
                using(var u = CreateUnitOfWork())
                {
                    foreach(var data in datas)
                    {
                        data.UpdateDate = DateTime.Now;
                         u.AcsItemInDetails.UpdateAcsItemInDetail(data);
                    }
                  
                    u.Complete();
                    return ObjectResult.Succeed(datas);
                }
            }catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }


        public ObjectResult DeleteAcsItemIn(AcsItemIn entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AcsItemIns.Edit(entity);
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
            }
            catch (Exception ex)
            {

                return ObjectResult.Fail(ex);
            }
        }
    }
}
