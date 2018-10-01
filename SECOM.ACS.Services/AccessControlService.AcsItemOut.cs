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
        public AcsItemOut GetAcsItemOut(string requestNo, LoadAcsItemOutOptions loadOptions)
        {
            using (var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsItemOuts.Get(requestNo);
                LoadData(u, dataItem, loadOptions);
                return dataItem;
            }
        }

        private void LoadData(IUnitOfWork unitOfWork, AcsItemOut acs, LoadAcsItemOutOptions loadOptions)
        {
            if (acs == null) { return; }

            if ((loadOptions & LoadAcsItemOutOptions.RequestEmployee) > 0)
            {
                acs.RequestEmployee = unitOfWork.Employees.GetInformation(acs.CreateBy);
            }

            acs.ForceDoneEmployee = unitOfWork.Employees.GetInformation(acs.ForceDoneBy);
            //acs.GAEmployee = unitOfWork.Employees.GetInformation(acs.GAApproveUserName);

            if ((loadOptions & LoadAcsItemOutOptions.Approval) > 0)
            {
                acs.ReqApproverList = unitOfWork.ReqApprovers.Find(t => String.Compare(t.ReqNo, acs.ReqNo, true) == 0)
                     .OrderBy(t => t.Step)
                     .ToList();
            }

            acs.Purpose = unitOfWork.Miscs.Get(acs.PurposeCodeID);
            acs.Area = unitOfWork.Areas.Get(acs.AreaID);
        }

        public ObjectResult CreateAcsItemOut(AcsItemOut entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    //Insert AcsItemOut
                    unitOfWork.AcsItemOuts.InsertAcsItemOut(entity);

                    //Loop Insert AcsItemOutDetail
                    foreach (var data in entity.AcsItemOutDetail)
                    {
                        data.ReqNo = entity.ReqNo;
                        data.UpdateBy = entity.CreateBy;
                        unitOfWork.AcsItemOutDetails.InsertAcsItemOutDetail(data);
                    }

                    //Loop Insert Approve Data
                    foreach (var dataApprove in entity.ReqApproverList)
                    {
                        dataApprove.ReqNo = entity.ReqNo;
                        unitOfWork.ReqApprovers.Add(dataApprove);
                    }

                    unitOfWork.Complete();
                    return ObjectResult.Succeed(entity);
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateAcsItemOutForAuthor(AcsItemOut entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    //Insert AcsItemOut
                    unitOfWork.AcsItemOuts.UpdateAcsItemOut(entity);

                    //Delete Old AcsItemOutDetail
                    unitOfWork.AcsItemOutDetails.DeleteAcsItemOutDetail(entity.ReqNo);
                    //Loop Insert AcsItemOutDetail
                    foreach (var data in entity.AcsItemOutDetail)
                    {
                        data.UpdateBy = entity.UpdateBy;
                        unitOfWork.AcsItemOutDetails.InsertAcsItemOutDetail(data);
                    }

                    unitOfWork.Complete();
                    return ObjectResult.Succeed(entity);
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateAcsItemOut(AcsItemOut entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsItemOuts.Edit(entity);

                    //Loop Insert AcsItemOutDetail
                    foreach (var data in entity.AcsItemOutDetail)
                    {
                        u.AcsItemOutDetails.UpdateAcsItemOutDetail(data);
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

        public IEnumerable<AcsItemOutDetailDataView> GetAcsItemOutDetailDataViews(string requestNo)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.AcsItemOutDetails.GetDataViews(requestNo).ToList();
            }
        }

        public ObjectResult DeleteAcsItemOut(AcsItemOut entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AcsItemOuts.Remove(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }
    }
}
