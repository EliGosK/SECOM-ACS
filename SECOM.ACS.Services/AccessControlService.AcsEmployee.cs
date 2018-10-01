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
        public AcsEmployee GetAcsEmployee(string requestNo,LoadAcsEmployeeOptions loadOptions)
        {
            using (var u = CreateUnitOfWork())
            {
                var dataItem = u.AcsEmployees.Get(requestNo);
                LoadData(u, dataItem, loadOptions);
                return dataItem;
            }
        }

        private void LoadData(IUnitOfWork unitOfWork, AcsEmployee acs, LoadAcsEmployeeOptions loadOptions)
        {
            if (acs == null) { return; }

            if ((loadOptions & LoadAcsEmployeeOptions.RequestEmployee) > 0)
            {
                acs.RequestEmployee = unitOfWork.Employees.GetInformation(acs.CreateBy);
            }

            if ((loadOptions & LoadAcsEmployeeOptions.EmployeeDetail) > 0)
            {
                acs.AcsEmployeeDetails = unitOfWork.AcsEmployeeDetails.Find(t => t.ReqNo == acs.ReqNo).ToList();
                if (acs.RequestFor == RequestFors.Employee) {
                    foreach (var detail in acs.AcsEmployeeDetails)
                    {
                        var employee = unitOfWork.Employees.GetInformation(detail.EmpID);
                        if (employee == null) { continue; }
                        detail.Employee = employee;
                    }
                }
            }

            if ((loadOptions & LoadAcsEmployeeOptions.Approval) > 0)
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

            if ((loadOptions & LoadAcsEmployeeOptions.RequestAreaMapping) > 0)
            {
                acs.ReqAreaMappings = unitOfWork.ReqAreaMappings.Find(t => t.ReqNo == acs.ReqNo).OrderBy(t=>t.AreaID).ToList();
            }

            if ((loadOptions & LoadAcsEmployeeOptions.Purpose) > 0)
            {
                acs.Purpose = unitOfWork.Miscs.Find(t => t.MiscID == acs.PurposeCodeID).FirstOrDefault();
            }
        }

        public ObjectResult CreateAcsEmployee(AcsEmployee entity)
        {
            bool acsInserted = false;
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    u.AcsEmployees.Add(entity);
                    acsInserted = true;
                    //Insert Employee Detail
                    foreach (var employee in entity.AcsEmployeeDetails)
                    {
                        employee.ReqNo = entity.ReqNo;
                        u.AcsEmployeeDetails.Add(employee);
                    }

                    // Insert Request Approver List
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
                        u.AcsEmployeeDetails.RemovesByRequestNo(entity.ReqNo);
                        u.AcsEmployees.Remove(entity);
                        u.Complete();
                    }
                    return ObjectResult.Fail(ex);
                }
            }
        }

        public ObjectResult UpdateAcsEmployee(AcsEmployee entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsEmployees.Edit(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {

                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateAcsEmployeeForAuthor(AcsEmployee entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.AcsEmployees.Update(entity);
                    u.AcsEmployeeDetails.RemovesByRequestNo(entity.ReqNo);
                    foreach (var detail in entity.AcsEmployeeDetails)
                    {
                        u.AcsEmployeeDetails.Add(detail);
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

        public ObjectResult DeleteAcsEmployee(AcsEmployee entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                    u.ReqAreaMappings.RemovesByRequestNo(entity.ReqNo);
                    u.AcsEmployeeDetails.RemovesByRequestNo(entity.ReqNo);
                    u.AcsEmployees.Remove(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public IEnumerable<AcsEmployeeDetailDataView> GetAcsEmployeeDetailDataViews(string requestNo)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsEmployeeDetails.GetDataViews(requestNo).ToList();
            }
        }
    }
}
