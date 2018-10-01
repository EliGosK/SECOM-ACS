using CSI.Core.Extensions;
using CSI.Localization;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static partial class ModelExtensions
    {
        public static AcsPhotoViewModel ToViewModel(this AcsPhoto entity)
        {
            var model = AutoMapper.Mapper.Map<AcsPhotoViewModel>(entity);
            
            var approver1 = entity.ReqApproverList.FirstOrDefault(t => t.Step == 1);
            if (approver1 != null)
            {
                model.SuperiorApprovalID = approver1.ApprovalID.ToString();
                model.SuperiorApproveUserName = approver1.ApproveUserName;
                model.SuperiorApprovalCode = approver1.ApprovalCode;
                model.SuperiorApprovalDate = approver1.ApprovalDate;
                model.SuperiorRejectReason = approver1.RejectReason;
            }

            var approver2 = entity.ReqApproverList.FirstOrDefault(t => t.Step == 2);
            if (approver2 != null)
            {
                model.AreaApprovalID = approver2.ApprovalID.ToString();
                model.AreaApproveUserName = approver2.ApproveUserName;
                model.AreaApprovalCode = approver2.ApprovalCode;
                model.AreaApprovalDate = approver2.ApprovalDate;
                model.AreaRejectReason = approver2.RejectReason;
            }

            model.AcknowledgeUserName = entity.AckBy;
            model.AcknowledgeDate = entity.AckDate;

            //model.ActReturnDate = model.ActReturnDate == DateTime.MinValue ? DateTime.Now : model.ActReturnDate;
            return model;
        }

        public static AcsPhoto ToEntity(this AcsPhotoViewModel viewModel)
        {
            var model = AutoMapper.Mapper.Map<AcsPhoto>(viewModel);
            model.Note = HttpUtility.HtmlDecode(viewModel.Note);
            model.ConvertMinDateToNull(t => t.ActReturnDate);
            // Superior
            if (!String.IsNullOrEmpty(viewModel.SuperiorApproveUserName))
            {
                model.ReqApproverList.Add(new ReqApproverList()
                {
                    ApprovalID = String.IsNullOrEmpty(viewModel.SuperiorApprovalID) ? Guid.NewGuid() : Guid.Parse(viewModel.SuperiorApprovalID),
                    ReqNo = viewModel.ReqNo,
                    Step = Convert.ToByte(1),
                    ApproveUserName = viewModel.SuperiorApproveUserName,
                    AreaID = viewModel.AreaID,
                    ApprovalCode = viewModel.SuperiorApprovalCode,
                    ApprovalDate = viewModel.SuperiorApprovalDate,
                    RejectReason = viewModel.SuperiorRejectReason,
                    CreateBy = viewModel.CreateBy,
                    UpdateBy = viewModel.UpdateBy,
                    ReqType = AcsRequestTypes.Photographing
                });
            }

            // Area
            model.ReqApproverList.Add(new ReqApproverList()
            {
                ApprovalID = String.IsNullOrEmpty(viewModel.AreaApprovalID) ? Guid.NewGuid() : Guid.Parse(viewModel.AreaApprovalID),
                ReqNo = viewModel.ReqNo,
                Step = Convert.ToByte(2),
                ApproveUserName = viewModel.AreaApproveUserName,
                AreaID = viewModel.AreaID,
                ApprovalCode = viewModel.AreaApprovalCode,
                ApprovalDate = viewModel.AreaApprovalDate,
                RejectReason = viewModel.AreaRejectReason,
                CreateBy = viewModel.CreateBy,
                UpdateBy = viewModel.UpdateBy,
                ReqType = AcsRequestTypes.Photographing
            });
            model.AckBy = viewModel.AcknowledgeUserName;
            model.AckDate = viewModel.AcknowledgeDate;
            return model;
        }

        public static AcsViewModel ToDataView(this AcsPhoto viewModel)
        {
            return AutoMapper.Mapper.Map<AcsViewModel>(viewModel);
        }
    }
}