using CSI.Core.Extensions;
using CSI.Localization;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static partial class ModelExtensions
    {
        public static AcsEmployeeViewModel ToViewModel(this AcsEmployee model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsEmployeeViewModel>(model);
            // Superior Approval
            var superior = model.ReqApproverList.FirstOrDefault(t => t.Step == 1);
            if (superior != null)
            {
                viewModel.SuperiorApprovalID = superior.ApprovalID.ToString();
                viewModel.SuperiorApproveUserName = superior.ApproveUserName;
                viewModel.SuperiorRejectReason = superior.RejectReason;
                viewModel.SuperiorApprovalCode = superior.ApprovalCode;
                viewModel.SuperiorApprovalDate = superior.ApprovalDate;
                viewModel.SuperiorEmployee = (superior.ApproveEmployee ?? new Employee()).ToViewModel();
                viewModel.SuperiorReferenceApprovalID = superior.ReferenceApprovalID.HasValue ? superior.ReferenceApprovalID.Value.ToString() : null;
            }

            // Convert ReqApproverList => ReqApproverListViewModel
            model.ReqApproverList.Where(t => t.Step > 1)
                .ToList()
                .ForEach((ReqApproverList approver) => viewModel.AreaApprovals.Add(approver.ToViewModel()));

            // Selected Area
            viewModel.Areas = model.ReqAreaMappings.OrderBy(t=>t.AreaID).Select(t => t.AreaID).ToArray();
            
            return viewModel;
        }

        /// <summary>
        /// Convert AcsEmployeeViewModel to AcsEmployee
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static AcsEmployee ToEntity(this AcsEmployeeViewModel viewModel)
        {
            var entity = AutoMapper.Mapper.Map<AcsEmployee>(viewModel);
            entity.Note = HttpUtility.HtmlDecode(viewModel.Note);
            // Employee Detail
            foreach (var employeeDetail in viewModel.AcsEmployeeDetails)
            {
                switch (viewModel.RequestFor)
                {
                    case RequestFors.BusinessTrip:
                        entity.AcsEmployeeDetails.Add(new AcsEmployeeDetail() { ReqNo = entity.ReqNo,  Seq = Convert.ToByte(employeeDetail.Seq), Name = employeeDetail.EmployeeName, DeptName = employeeDetail.DepartmentName });
                        break;
                    case RequestFors.Employee:
                        entity.AcsEmployeeDetails.Add(new AcsEmployeeDetail() { ReqNo = entity.ReqNo, Seq = Convert.ToByte(employeeDetail.Seq), EmpID = employeeDetail.EmployeeID });
                        break;
                    default:
                        entity.AcsEmployeeDetails.Add(new AcsEmployeeDetail() { ReqNo = entity.ReqNo, Seq = Convert.ToByte(employeeDetail.Seq), EmpID = employeeDetail.EmployeeID, Name = employeeDetail.EmployeeName, DeptName = employeeDetail.DepartmentName });
                        break;
                }
            }

            // Req Area Mapping
            foreach (var areaApproval in viewModel.AreaApprovals)
            {
                if (!areaApproval.AreaID.HasValue) { continue; }
                entity.ReqAreaMappings.Add(new ReqAreaMapping() { AreaID = areaApproval.AreaID.Value });
            }

            if (!String.IsNullOrEmpty(viewModel.SuperiorApproveUserName))
            {
                // Req Approver List
                entity.ReqApproverList.Add(new ReqApproverList()
                {
                    ApprovalID = String.IsNullOrEmpty(viewModel.SuperiorApprovalID) ? Guid.NewGuid() : Guid.Parse(viewModel.SuperiorApprovalID),
                    ReqNo = viewModel.ReqNo,
                    Step = 1,
                    ApproveUserName = viewModel.SuperiorApproveUserName,
                    ApprovalCode = viewModel.SuperiorApprovalCode,
                    ApprovalDate = viewModel.SuperiorApprovalDate,
                    RejectReason = viewModel.SuperiorRejectReason,
                    CreateBy = viewModel.CreateBy,
                    UpdateBy = viewModel.UpdateBy,
                });
            }

            var step = 2;
            foreach (var areaApproval in viewModel.AreaApprovals)
            {
                if (!areaApproval.AreaID.HasValue) { continue; }
                entity.ReqApproverList.Add(new ReqApproverList()
                {
                    ApprovalID = String.IsNullOrEmpty(viewModel.SuperiorApprovalID) ? Guid.NewGuid() : Guid.Parse(viewModel.SuperiorApprovalID),
                    ReqNo = viewModel.ReqNo,
                    Step = areaApproval.Step == 0 ? Convert.ToByte(step) : areaApproval.Step,
                    AreaID = areaApproval.AreaID,
                    ApproveUserName = areaApproval.ApproveUserName,
                    ApprovalCode = areaApproval.ApprovalCode,
                    ApprovalDate = areaApproval.ApprovalDate,
                    RejectReason = areaApproval.RejectReason,
                    CreateBy = viewModel.CreateBy,
                    UpdateBy = viewModel.UpdateBy,
                });
            }
            return entity;
        }

        public static AcsViewModel ToDataView(this AcsEmployee model)
        {
            return AutoMapper.Mapper.Map<AcsViewModel>(model);
        }

        public static AcsEmployeeDetailViewModel ToViewModel(this AcsEmployeeImportData entity)
        {
            return new AcsEmployeeDetailViewModel()
            {
                DepartmentName = entity.DepartmentName,
                EmployeeID = entity.EmployeeID,
                EmployeeName = entity.EmployeeName
            };
        }

        public static AcsEmployeeDetailViewModel ToViewModel(this AcsEmployeeDetail entity)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsEmployeeDetail, AcsEmployeeDetailViewModel>(entity);
            if (entity.Employee != null) {
                viewModel.EmployeeName = ModelLocalizeManager.GetValue(entity.Employee, "EmpName");
                viewModel.DepartmentName = ModelLocalizeManager.GetValue(entity.Employee, "DeptName");
            }
            return viewModel;
        }

        public static AcsEmployeeDetailViewModel ToViewModel(this AcsEmployeeDetailDataView dataView)
        {
            return new AcsEmployeeDetailViewModel()
            {
                DetailID = dataView.DetailID,
                Seq = dataView.Seq,
                EmployeeID = dataView.EmpID,
                DepartmentName = dataView.DeptName ?? ModelLocalizeManager.GetValue(dataView, "DepartmentName"),
                EmployeeName = dataView.Name ?? ModelLocalizeManager.GetValue(dataView, "EmpName")
            };
        }

       
    }
}