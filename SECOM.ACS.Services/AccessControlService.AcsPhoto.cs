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
        public AcsPhoto GetAcsPhoto(string requestNo, LoadAcsPhotoOptions loadOptions)
        {
            using (var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsPhotos.Get(requestNo);
                LoadData(u, dataItem, loadOptions);
                return dataItem;
            }
        }

        private void LoadData(IUnitOfWork unitOfWork, AcsPhoto acs, LoadAcsPhotoOptions loadOptions)
        {
            if (acs == null) { return; }

            if ((loadOptions & LoadAcsPhotoOptions.RequestEmployee) > 0)
            {
                acs.RequestEmployee = unitOfWork.Employees.GetInformation(acs.CreateBy);
            }

            if (acs.PhotoByType == PhotoByTypes.Employee)
            {
                if ((loadOptions & LoadAcsPhotoOptions.PhotoEmployee) > 0)
                {
                    acs.PhotoEmployee = unitOfWork.Employees.Get(acs.PhotoEmpID);
                }
            }

            if ((loadOptions & LoadAcsPhotoOptions.PhotoWitness) > 0)
            {
                acs.WitnessEmployee = unitOfWork.Employees.Get(acs.WitnessEmpID);
            }

            if ((loadOptions & LoadAcsPhotoOptions.Approval) > 0)
            {
                acs.ReqApproverList = unitOfWork.ReqApprovers.Find(t => t.ReqNo == acs.ReqNo).ToList();
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

            if ((loadOptions & LoadAcsPhotoOptions.Area) > 0)
            {
                acs.Area = unitOfWork.Areas.Get(acs.AreaID);
            }

            if ((loadOptions & LoadAcsPhotoOptions.Purpose) > 0)
            {
                acs.Purpose = unitOfWork.Miscs.Find(t => t.MiscID == acs.PurposeCodeID).FirstOrDefault();
            }

            if ((loadOptions & LoadAcsPhotoOptions.EquipItem) > 0)
            {
                acs.Item = unitOfWork.Items.Get(acs.EquipItemID);
            }

            if (!String.IsNullOrEmpty(acs.ForceDoneBy))
            {
                acs.ForceDoneEmployee = unitOfWork.Employees.GetByUserName(acs.ForceDoneBy);
            }
        }

        public ObjectResult CreateAcsPhoto(AcsPhoto entity)
        {
            bool acsInserted = false;
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    u.AcsPhotos.Add(entity);
                    acsInserted = true;
                    foreach (var approver in entity.ReqApproverList)
                    {
                        approver.ReqNo = entity.ReqNo;
                        u.ReqApprovers.Add(approver);
                    }
                    u.Complete();
                    return ObjectResult.Succeed(entity);
                }
                catch (Exception ex)
                {
                    if (acsInserted) {
                        u.AcsPhotos.Remove(entity);
                        u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                        u.Complete();
                    }
                    return ObjectResult.Fail(ex);
                }
            }
        }

        public ObjectResult UpdateAcsPhoto(AcsPhoto entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsPhotos.Edit(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {

                return ObjectResult.Fail(ex);
            }
           
        }

        public ObjectResult DeleteAcsPhoto(AcsPhoto entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AcsPhotos.Remove(entity);
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
