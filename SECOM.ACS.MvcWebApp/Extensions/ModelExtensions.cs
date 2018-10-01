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
        #region Misc
        public static MiscViewModel ToViewModel(this Misc model)
        {
            var entity = AutoMapper.Mapper.Map<MiscViewModel>(model);
            entity.MiscDisplay = ModelLocalizeManager.GetValue(model, "MiscDisplay");
            return entity;
        }

        public static Misc ToEntity(this MiscViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<Misc>(model);
            return entity;
        }
        #endregion

        #region Area
        public static AreaViewModel ToViewModel(this Area model)
        {
            var entity = AutoMapper.Mapper.Map<AreaViewModel>(model);
            //entity.AreaName = ModelLocalizeManager.GetValue(model, "AreaDisplay");
            entity.AreaName = model.AreaName;
            entity.SelectedGates = model.Gates.Select(t => t.GateID).ToArray();
            foreach (var app in model.AreaApprovers)
            {
                if (app.Seq == 1)
                {
                    entity.ApproverDepartment = app.DepartmentID;
                    entity.ApproverPosition = app.PositionID;
                }
                else if (app.Seq == 2)
                {
                    entity.Sub1Department = app.DepartmentID;
                    entity.Sub1Position = app.PositionID;
                }
                else
                {
                    entity.Sub2Department = app.DepartmentID;
                    entity.Sub2Position = app.PositionID;
                }
            }
            return entity;
        }

        public static Area ToEntity(this AreaViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<Area>(model);
            if (model.ApproverDepartment != null && model.ApproverPosition != null)
            {
                entity.AreaApprovers.Add(new AreaApprover()
                {
                    Seq = 1,
                    AreaID = model.AreaID,
                    DepartmentID = model.ApproverDepartment,
                    PositionID = model.ApproverPosition
                }
                );
            }
            if (model.Sub1Position != null && model.Sub1Department != null)
            {
                entity.AreaApprovers.Add(new AreaApprover()
                {
                    Seq = 2,
                    AreaID = model.AreaID,
                    DepartmentID = model.Sub1Department,
                    PositionID = model.Sub1Position
                }
              );
            }
            if (model.Sub2Position != null && model.Sub2Department != null)
            {
                entity.AreaApprovers.Add(new AreaApprover()
                {
                    Seq = 3,
                    AreaID = model.AreaID,
                    DepartmentID = model.Sub2Department,
                    PositionID = model.Sub2Position
                }
              );
            }
            return entity;
        }

        public static AreaMappingViewModel ToViewModel(this AreaMapping model)
        {
            return new AreaMappingViewModel
            {
                AreaID = model.AreaID,
                FactoryCode = AcsModelHelper.GetFactory(model.FactoryCode),
                AreaName = ModelLocalizeManager.GetValue(model, "AreaDisplay"),
                IsMainArea = model.IsMainArea
            };
        }

        public static AreaMapping ToAreaMapping(this Area area)
        {
            return new AreaMapping()
            {
                AreaID = area.AreaID,
                AreaDisplayEN = area.AreaDisplayEN,
                AreaDisplayTH = area.AreaDisplayTH,
            };
        }

        public static AreaMappingViewModel ToMappingViewModel(this Area area)
        {
            return new AreaMappingViewModel()
            {
                AreaID = area.AreaID,
                FactoryCode = AcsModelHelper.GetFactory(area.FactoryCode),
                AreaName = ModelLocalizeManager.GetValue( area,"AreaDisplay"),
                IsMainArea = false
            };
        }

        public static AreaDataViewModel ToDataViewModel(this Area model)
        {
            return new AreaDataViewModel() { AreaID = model.AreaID, FactoryCode = model.FactoryCode, AreaDisplay = ModelLocalizeManager.GetValue(model, "AreaDisplay") };
        }
        #endregion

        #region Item
            public static ItemDataViewModel ToViewModel(this ItemDataView dataView)
        {
            var model = AutoMapper.Mapper.Map<ItemDataViewModel>(dataView);
            model.MiscDisplay = ModelLocalizeManager.GetValue(dataView, "MiscDisplay");
            return model;
        }

        public static ItemViewModel ToViewModel(this Item dataView)
        {
            var model = AutoMapper.Mapper.Map<ItemViewModel>(dataView);
            return model;
        }

        public static Item ToEntity(this ItemDataViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<Item>(model);
            return entity;
        }
        #endregion

        #region Card
        public static CardViewModel ToViewModel(this Card dataView)
        {
            var model = AutoMapper.Mapper.Map<CardViewModel>(dataView);
            model.SelectedArea = dataView.AreaCardMappings.Select(t => t.AreaID).ToArray();
            dataView.AreaCardMappings.Select(t => t.Area).ToList()
                .ForEach((Area area) => model.Area.Add(area.ToDataViewModel()));
            return model;
        }

        public static Card ToEntity(this CardViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<Card>(model);
            foreach (var item in model.Area)
            {
                entity.AreaCardMappings.Add(new AreaCardMapping() { CardID = model.CardID, AreaID = item.AreaID });
            }
            return entity;
        }

        public static Card ToEntity(this CardImportData model, string user)
        {
            var entity = AutoMapper.Mapper.Map<Card>(model);
            entity.CreateBy = user;
            entity.UpdateBy = user;
            return entity;
        }
        #endregion

        #region Request Approver List
        public static ReqApproverListViewModel ToViewModel(this ReqApproverList entity)
        {
            var viewModel = AutoMapper.Mapper.Map<ReqApproverList, ReqApproverListViewModel>(entity);
            viewModel.ApproveEmployee = (entity.ApproveEmployee ?? new Employee()).ToViewModel();
            return viewModel;
        }

        public static ReqApproverList Clone(this ReqApproverList entity)
        {
            return AutoMapper.Mapper.Map<ReqApproverList, ReqApproverList>(entity);
            
        }
        #endregion

        #region Request Visitor
        public static AcsVisitorViewModel ToViewModel(this AcsVisitor model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsVisitorViewModel>(model);
            // Superior Approval
            var superior = model.ReqApproverList.OrderByDescending(t=>t.CreateDate)
                .FirstOrDefault(t => t.Step == 1);
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
            viewModel.Areas = model.ReqAreaMappings.Select(t => t.AreaID).ToArray();
            return viewModel;
        }

        public static AcsVisitor ToEntity(this AcsVisitorViewModel viewModel)
        {
            var entity = AutoMapper.Mapper.Map<AcsVisitor>(viewModel);
            entity.Note = HttpUtility.HtmlDecode(viewModel.Note);
            // Acs Visitor Detail
            foreach (var visitor in viewModel.AcsVisitorDetails)
            {
                var v = visitor.ToEntity();
                v.ReqNo = entity.ReqNo;
                entity.AcsVisitorDetails.Add(v);
            }

            // Req Area Mappings
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

        [Obsolete]
        public static AcsViewModel ToDataView(this AcsVisitor viewModel)
        {
            return AutoMapper.Mapper.Map<AcsViewModel>(viewModel);
        }

        public static AcsVisitorDetailViewModel ToViewModel(this AcsVisitorImportData model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsVisitorImportData, AcsVisitorDetailViewModel>(model);
            return viewModel;
        }

        public static AcsVisitorDetail ToEntity(this AcsVisitorDetailViewModel viewModel)
        {
            var entity = AutoMapper.Mapper.Map<AcsVisitorDetailViewModel, AcsVisitorDetail>(viewModel);
            return entity;
        }

        public static AcsVisitorDetailViewModel ToViewModel(this AcsVisitorDetailDataView dataView)
        {
            return new AcsVisitorDetailViewModel()
            {
                DetailID = dataView.DetailID,
                Seq = dataView.Seq,
                VerifyNo = dataView.VerifyNo,
                VerifyTypeID = dataView.VerifyTypeID.HasValue ? dataView.VerifyTypeID.Value : 0,
                VerifyType = dataView.VerifyTypeID == 0 ? "" : ModelLocalizeManager.GetValue(dataView, "VerifyType"),
                Company = dataView.Company,
                DepartmentName = dataView.DeptName,
                Description = dataView.Description,
                ItemInOut = dataView.ItemInOut,
                Photographing = dataView.Photographing,
                VisitorName = dataView.Name
            };
        }

        public static AcsVisitorTransactionDataViewModel ToViewModel(this AcsVisitorTransactionDataView dataview)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsVisitorTransactionDataViewModel>(dataview);
            return viewModel;
        }
        #endregion

        #region Request Item-In
        public static AcsItemInDetailViewModel ToViewModel(this AcsItemInDetailDataView model) {
            var viewModel = AutoMapper.Mapper.Map<AcsItemInDetailViewModel>(model);
            viewModel.Confidential = model.ConfdtFlag;
            viewModel.ItemName = ModelLocalizeManager.GetValue(model, "ItemDisplay");
            viewModel.ItemTypeName = ModelLocalizeManager.GetValue(model, "ItemTypeDisplay");
            return viewModel;
        }

        public static AcsItemInDetailViewModel ToViewModel(this AcsItemInDetail model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsItemInDetailViewModel>(model);
            viewModel.ItemName = ModelLocalizeManager.GetValue(model, "ItemDisplay");
            viewModel.ItemTypeName = ModelLocalizeManager.GetValue(model, "ItemTypeDisplay");
            return viewModel;
        }

        public static AcsItemInViewModel ToViewModel(this AcsItemIn model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsItemInViewModel>(model);
            //SuperiorApprove
            var superior =  model.ReqApproverList.FirstOrDefault(t => t.Step == 1);
            if (superior != null) {
                viewModel.SuperiorApprovalID = superior.ApprovalID;
                viewModel.SuperiorApproveUserName = superior.ApproveUserName;
                viewModel.SuperiorApprovalCode = superior.ApprovalCode;
                viewModel.SuperiorApprovalDate = superior.ApprovalDate;
                viewModel.SuperiorRejectReason = superior.RejectReason;
            }

            var area = model.ReqApproverList.FirstOrDefault(t => t.Step == 2);
            if (area != null)
            {
                viewModel.AreaApprovalID = area.ApprovalID;
                viewModel.AreaApproveUserName = area.ApproveUserName;
                viewModel.AreaApprovalCode = area.ApprovalCode;
                viewModel.AreaApprovalDate = area.ApprovalDate;
                viewModel.AreaRejectReason = area.RejectReason;
            }
        
           
            viewModel.AcknowledgeUserName = model.AckBy;

            viewModel.AcknowledgeDate  = model.AckDate;
            return viewModel;
        }

        public static AcsItemIn ToEntity(this AcsItemInViewModel viewModel)
        {
            var model = AutoMapper.Mapper.Map<AcsItemIn>(viewModel);
            model.Note = HttpUtility.HtmlDecode(viewModel.Note);
            foreach (var data in viewModel.AcsItemInDetails)
            {
                model.AcsItemInDetails.Add(new AcsItemInDetail()
                {
                    DetailID = data.DetailID,
                    ItemID = data.ItemID,
                    ReqNo = model.ReqNo,
                    Seq = data.Seq,
                    Description = data.Description,
                    ConfdtFlag = data.Confidential,
                    ReqQty = data.RequestItemQty,
                    ActReturnDate = data.ActualReturnDate,
                    ActReturnQty = data.ActualReturnQty,
                    PlanReturnDate = data.PlanReturnDate,
                    UpdateBy = data.UpdateBy,
                    UpdateDate = DateTime.Now,
                    ActInQty = data.ActualTakeInQty,
                });
            }
            // Superior
            model.ReqApproverList.Add(new ReqApproverList()
            {
                ApprovalID = viewModel.SuperiorApprovalID==Guid.Empty ? Guid.NewGuid() : viewModel.SuperiorApprovalID,
                ReqNo = viewModel.ReqNo,
                Step = Convert.ToByte(model.ReqApproverList.Count() + 1),
                ApproveUserName = viewModel.SuperiorApproveUserName,
                AreaID = viewModel.AreaID,
                ApprovalCode = viewModel.SuperiorApprovalCode,
                ApprovalDate = viewModel.SuperiorApprovalDate,
                RejectReason = viewModel.SuperiorRejectReason,
                CreateBy = viewModel.CreateBy,
                UpdateBy = viewModel.UpdateBy,
                ReqType = AcsRequestTypes.Photographing
            });

            // Area
            model.ReqApproverList.Add(new ReqApproverList()
            {
                ApprovalID = viewModel.AreaApprovalID==Guid.Empty ? Guid.NewGuid() : viewModel.AreaApprovalID,
                ReqNo = viewModel.ReqNo,
                Step = Convert.ToByte(model.ReqApproverList.Count() + 1),
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
            model.TakeInDate = viewModel.TakeInDate;
            model.CreateBy = viewModel.CreateBy;
            model.CreateDate = viewModel.CreateDate;

            return model;
        }
               
        #endregion

        #region Item Out
        public static AcsItemOutViewModel ToViewModel(this AcsItemOut model)
        {

            var viewModel = AutoMapper.Mapper.Map<AcsItemOutViewModel>(model);

            //Flag Show Column in Grid
            viewModel.IsShowActualItem = true;

           
            var superiorApprover = model.ReqApproverList.FirstOrDefault(t => t.Step == 1);
            if (superiorApprover != null) {
                viewModel.SuperiorApprovalID = superiorApprover.ApprovalID;
                viewModel.SuperiorApproveUserName = superiorApprover.ApproveUserName;
                viewModel.SuperiorApprovalCode = superiorApprover.ApprovalCode;
                viewModel.SuperiorApprovalDate = superiorApprover.ApprovalDate;
                viewModel.SuperiorRejectReason = superiorApprover.RejectReason;
            }

            var gaApprover = model.ReqApproverList.FirstOrDefault(t => t.Step > 1);
            if (gaApprover != null)
            {
                viewModel.GAApprovalID = gaApprover.ApprovalID;
                viewModel.GAApproveUserName = gaApprover.ApproveUserName;
                viewModel.GAApprovalCode = gaApprover.ApprovalCode;
                viewModel.GAApprovalDate = gaApprover.ApprovalDate;
                viewModel.GARejectReason = gaApprover.RejectReason;
            }

            if(model.RequestEmployee != null)
            {
                viewModel.RequestEmployee = model.RequestEmployee.ToViewModel();
            }

            return viewModel;
        }

        public static AcsItemOut ToEntity(this AcsItemOutViewModel viewModel)
        {
            var entity = AutoMapper.Mapper.Map<AcsItemOut>(viewModel);
            entity.Note = HttpUtility.HtmlDecode(viewModel.Note);
            foreach (var itemList in viewModel.AcsItemOutDetails)
            {
                if(itemList.DetailID == null)
                {
                    itemList.DetailID = Guid.NewGuid().ToString();
                }

                entity.AcsItemOutDetail.Add(new AcsItemOutDetail()
                {
                    DetailID = new Guid(itemList.DetailID),
                    ReqNo = entity.ReqNo,
                    Seq = itemList.Seq,
                    ItemID = itemList.ItemID,
                    Description = itemList.Description,
                    ConfdtFlag = itemList.Confidential,
                    ReqQty = itemList.RequestItemQty,
                    PlanReturnDate = itemList.PlanReturnDate,
                    ActOutQty = itemList.ActualTakeOutQty,
                    ActReturnQty = itemList.ActualReturnQty,
                    ActReturnDate = itemList.ActualReturn,
                    UpdateBy = itemList.UpdateBy
                });
            }

            //Superior
            entity.ReqApproverList.Add(new ReqApproverList()
            {
                ReqNo = viewModel.RequestNo,
                Step = 1,
                ApproveUserName = viewModel.SuperiorApproveUserName,
                AreaID = null,
                CreateBy = viewModel.CreateBy
            });

            //GA
            entity.ReqApproverList.Add(new ReqApproverList()
            {
                ReqNo = viewModel.RequestNo,
                Step = 2,
                ApproveUserName = viewModel.GAApproveUserName,
                AreaID = null,
                CreateBy = viewModel.CreateBy
            });

            return entity;
        }

        public static AcsViewModel ToDataView(this AcsItemOut model)
        {
            return AutoMapper.Mapper.Map<AcsViewModel>(model);
        }

        public static AcsItemOutItemViewModel ToViewModel(this AcsItemOutDetailDataView dataView)
        {
            return new AcsItemOutItemViewModel()
            {
                DetailID = (dataView.DetailID).ToString(),
                ItemID = dataView.ItemID,
                Seq = dataView.Seq,
                Description = dataView.Description,
                Confidential = dataView.ConfdtFlag,
                RequestItemQty = dataView.ReqQty,
                PlanReturnDate = dataView.PlanReturnDate,
                ActualTakeOutQty = dataView.ActOutQty,
                ActualReturnQty = dataView.ActReturnQty,
                ActualReturn = dataView.ActReturnDate,
                ItemTypeID = dataView.Item.ItemTypeID,
                ItemName = ModelLocalizeManager.GetValue(dataView.Item, "ItemDisplay"),
                ItemTypeName = ModelLocalizeManager.GetValue(dataView.ItemType, "MiscDisplay"),
                UpdateBy = dataView.UpdateBy,
            };
        }
        #endregion

        #region TransactionAcs
        public static AcsTransactionViewModel ToViewModel(this TransactionAcs model)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsTransactionViewModel>(model);
            return viewModel;
        }
        #endregion

        #region Card
        public static VisitorCardRegistrationView ToEntity(this VisitorCardRegistrationViewModel model)
        {
            var entity = new VisitorCardRegistrationView();

            entity.TranID = model.TranID;
            entity.DetailID = model.DetailID;
            entity.CardID = model.CardID;
            entity.VerifyTypeID = model.VerifyTypeID;
            entity.VerifyNo = model.VerifyNo;

            return entity;
        }

        public static VisitorCardRegistrationViewModel ToViewModel(this VisitorCardRegistrationView dataView)
        {
            var model = AutoMapper.Mapper.Map<VisitorCardRegistrationViewModel>(dataView);
            model.VerifyTypeName = dataView.VerifyType==null?"": ModelLocalizeManager.GetValue(dataView.VerifyType, "MiscDisplay");
            model.CardNo = (dataView.Card == null) ? "" : dataView.Card.CardNo;
            model.OriginalCardID = model.CardID;
            model.AreaName = ModelLocalizeManager.GetValue(dataView, "AreaName");
            return model;
        }

        public static BusinessTripCardRegistrationView ToEntity(this BusinessTripCardRegistrationViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<BusinessTripCardRegistrationView>(model);
            entity.Card = new Card() { CardID = model.CardID, CardNo = model.CardNo };
            return entity;
        }

        public static BusinessTripCardRegistrationViewModel ToViewModel(this BusinessTripCardRegistrationView dataView)
        {
            var model = AutoMapper.Mapper.Map<BusinessTripCardRegistrationViewModel>(dataView);
            model.CardNo = (dataView.Card == null) ? "" : dataView.Card.CardNo;
            model.OriginalCardID = model.CardID;
            //model.IsAvailable = (dataView.CardID == null) ? true : false;
            model.IsAvailable = true;
            model.AreaName = ModelLocalizeManager.GetValue(dataView, "AreaName");
            model.EmployeeName = ModelLocalizeManager.GetValue(dataView, "EmpName");
            return model;
        }

        public static VIPCardRegistrationViewModel ToViewModel(this VIPCardRegistrationView dataView)
        {
            var model = AutoMapper.Mapper.Map<VIPCardRegistrationViewModel>(dataView);
            if (dataView.PositionType != null)
            {
                model.PositionName = (ModelLocalizeManager.GetValue(dataView.PositionType, "MiscDisplay") == null) ? "" : ModelLocalizeManager.GetValue(dataView.PositionType, "MiscDisplay");
            }

            model.SelectedAreaList = model.Area.Select(t => t.AreaID).ToArray();
            model.StatusName = ModelLocalizeManager.GetValue(dataView, "Status");
            return model;
        }

        public static AcsVIP ToEntity(this VIPCardRegistrationViewModel model)
        {
            var entity = AutoMapper.Mapper.Map<VIPCardRegistrationViewModel, AcsVIP>(model);
            entity.PositionMiscID = model.PositionID;
            if(model.Status == VIPCardStatus.Available)
            {
                foreach (var areaID in model.SelectedAreaList)
                {
                    if (areaID == 0) { continue; }
                    entity.ReqAreaMapping.Add(new ReqAreaMapping() { AreaID = areaID });
                }
            }
            
            return entity;
        }

        public static ReceiveReturnBusinessTripCardDataViewModel ToViewModel(this ReceiveReturnBusinessTripCardDataView model)
        {
            var viewModel = AutoMapper.Mapper.Map<ReceiveReturnBusinessTripCardDataViewModel>(model);
            
            return viewModel;

            //return new ReceiveReturnBusinessTripCardDataViewModel
            //{
            //    TranID = model.TranID,
            //    AreaName = ModelLocalizeManager.GetValue(model, "Area"),
            //    BusinessEmployeeName = model.BuseinssEmployee,
            //    CardID = model.CardID,
            //    CardNo = model.CardNo,
            //    DetailID = model.DetailID,
            //    EmployeeName = ModelLocalizeManager.GetValue(model, "EmpName"),
            //    EntryDateFrom = model.EntryDateFrom,
            //    EntryDateTo = model.EntryDateTo,
            //    TimeIn = model.TimeIn,
            //    TimeOut = model.TimeOut
            //};
        }
        public static ReceiveReturnBusinessTripCardDataView ToEntity(this ReceiveReturnBusinessTripCardDataViewModel model)
        {
            var viewModel = AutoMapper.Mapper.Map<ReceiveReturnBusinessTripCardDataView>(model);
            return viewModel;
            //return new ReceiveReturnBusinessTripCardDataView
            //{
            //    TranID = model.TranID,
            //    EntryDateFrom = model.EntryDateFrom,
            //    EntryDateTo = model.EntryDateTo,
            //    TimeIn = model.TimeIn,
            //    TimeOut = model.TimeOut,
            //    CardID = model.CardID,
            //    CardNo = model.CardNo,
            //    DetailID = model.DetailID
            //};
        }

        public static ReceiveReturnVisitorCardDataViewModel ToViewModel(this ReceiveReturnVisitorCardDataView model)
        {
            var viewModel = AutoMapper.Mapper.Map<ReceiveReturnVisitorCardDataViewModel>(model);
            viewModel.VerifyTypeName = model.VerifyType ==null ? "": ModelLocalizeManager.GetValue(model.VerifyType, "MiscDisplay");
            return viewModel;
        }

        public static ReceiveReturnVisitorCardDataView ToEntity(this ReceiveReturnVisitorCardDataViewModel model)
        {
            var viewModel = AutoMapper.Mapper.Map<ReceiveReturnVisitorCardDataView>(model);
            return viewModel;
        }
        #endregion

        #region Role
        public static RoleViewModel ToViewModel(this Role role)
        {
            return AutoMapper.Mapper.Map<RoleViewModel>(role);
        }

        public static Role ToEntity(this RoleViewModel model)
        {
            return AutoMapper.Mapper.Map<Role>(model);
        }

        public static UserRole ToUserRole(this Role role)
        {
            return AutoMapper.Mapper.Map<UserRole>(role);
        }
        #endregion

        public static AccountViewModel ToViewModel(this User user)
        {
            return AutoMapper.Mapper.Map<AccountViewModel>(user);
        }

        public static PermissionViewModel ToViewModel(this Permission dataView)
        {
            var model = new PermissionViewModel();
            model.RoleId = dataView.RoleId;
            model.DashboardAttributes = dataView.DashboardAttributes;
            foreach (var permission in dataView.Permissions)
            {
                permission.RoleID = dataView.RoleId;
                model.AuthorizeRules.Add(permission.Value);
            }
            return model;
        }

        #region AcsTask
        public static AcsTaskViewModel ToViewModel(this AcsTask entity)
        {
            var viewModel = AutoMapper.Mapper.Map<AcsTask, AcsTaskViewModel>(entity);

            return viewModel;
        }
        #endregion
    }
}