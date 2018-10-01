using AutoMapper;
using CSI.Localization;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.IO;
using System.Linq;

namespace SECOM.ACS.MvcWebApp
{
    public class MappingConfig
    {
        public static void Register()
        {
            AutoMapper.Mapper.Initialize(
                c =>
                {
                    c.CreateMap<Area, AreaViewModel>()
                        .ForMember(t => t.Status, opt => opt.MapFrom(src => src.IsActive ? "1" : "0"));

                    c.CreateMap<AreaDataView, AreaDataViewModel>()
                        .ForMember(t => t.AreaDisplay, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "AreaDisplay")));

                    c.CreateMap<AreaViewModel, Area>()
                        .ForMember(t => t.IsActive, opt => opt.MapFrom(src => src.Status == "1"));


                    c.CreateMap<Card, CardViewModel>()
                        .ForMember(t => t.IsActive, opt => opt.MapFrom(src => src.IsActive ? "1" : "0"));
                    c.CreateMap<CardViewModel, Card>()
                       .ForMember(t => t.IsActive, opt => opt.MapFrom(src => src.IsActive == "1"));
                    c.CreateMap<CardImportData, Card>();

                    c.CreateMap<Employee, EmployeeViewModel>()
                        .ForMember(t => t.EmployeeID, opt => opt.MapFrom(m => m.EmpID))
                        .ForMember(t => t.EmployeeName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "EmpName")));

                    c.CreateMap<EmployeeViewModel, Employee>()
                        .ForMember(t => t.EmpID, opt => opt.MapFrom(m => m.EmployeeID));

                    c.CreateMap<EmployeeInformation, EmployeeInformationViewModel>()
                        .ForMember(t => t.EmployeeID, opt => opt.MapFrom(m => m.EmpID))
                        .ForMember(t => t.IsLending, opt => opt.MapFrom(m => m.LendingFlag));

                    c.CreateMap<EmployeeInformationViewModel, EmployeeInformation>()
                        .ForMember(t => t.EmpID, opt => opt.MapFrom(m => m.EmployeeID))
                        .ForMember(t => t.LendingFlag, opt => opt.MapFrom(m => m.IsLending));

                    c.CreateMap<EmployeeApprovalDataView, EmployeeApprovalDataViewModel>()
                        .ForMember(t => t.EmployeeID, opt => opt.MapFrom(src => src.EmpID))
                        .ForMember(t => t.EmployeeName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "EmpName")))
                        .ForMember(t => t.PositionName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Position")));

                    c.CreateMap<EmployeeDataView, EmployeeDataViewModel>()
                        .ForMember(t => t.EmployeeID, opt => opt.MapFrom(src => src.EmpID))
                        .ForMember(t => t.EmployeeName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "EmpName")))
                        .ForMember(t => t.PositionName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Position")))
                        .ForMember(t => t.DepartmentName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Department")));

                    c.CreateMap<Gate, GateViewModel>();
                    c.CreateMap<GateViewModel, Gate>();


                    c.CreateMap<Item, ItemViewModel>()
                        .ForMember(t => t.ItemDisplay, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "ItemDisplay")));
                    c.CreateMap<ItemDataView, ItemDataViewModel>()
                       .ForMember(t => t.Status, opt => opt.MapFrom(src => src.IsActive ? "1" : "0"));
                    c.CreateMap<ItemDataViewModel, Item>()
                        .ForMember(t => t.IsActive, opt => opt.MapFrom(src => src.Status == "1"));

                    c.CreateMap<Misc, MiscViewModel>()
                         .ForMember(t => t.Status, opt => opt.MapFrom(src => src.IsActive ? "1" : "0"));
                    c.CreateMap<MiscViewModel, Misc>()
                        .ForMember(t => t.IsActive, opt => opt.MapFrom(src => src.Status == "1"));

                    c.CreateMap<VisitorCardRegistrationView, VisitorCardRegistrationViewModel>();

                    c.CreateMap<BusinessTripCardRegistrationView, BusinessTripCardRegistrationViewModel>()
                        .ForMember(t => t.BusinessEmployeeName, opt => opt.MapFrom(src => src.BusinessEmployee));

                    c.CreateMap<BusinessTripCardRegistrationViewModel, BusinessTripCardRegistrationView>()
                        .ForMember(t => t.BusinessEmployee, opt => opt.MapFrom(src => src.BusinessEmployeeName));

                    c.CreateMap<VIPCardRegistrationView, VIPCardRegistrationViewModel>()
                        .ForMember(t => t.RequestNo, opt => opt.MapFrom(src => src.ReqNo));

                    c.CreateMap<VIPCardRegistrationViewModel, AcsVIP>()
                        .ForMember(t => t.ReqNo, opt => opt.MapFrom(src => src.RequestNo))
                        .ForMember(t => t.Area, opt => opt.Ignore());

                    c.CreateMap<ReceiveReturnVisitorCardDataView, ReceiveReturnVisitorCardDataViewModel>()
                        .ForMember(t => t.VerifyTypeID, opt => opt.MapFrom(src => src.VerifyTypeID.HasValue ? src.VerifyTypeID.Value : 0))
                        .ForMember(t => t.AreaName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Area")));

                    c.CreateMap<ReceiveReturnVisitorCardDataViewModel, ReceiveReturnVisitorCardDataView>();

                    c.CreateMap<ReceiveReturnBusinessTripCardDataView, ReceiveReturnBusinessTripCardDataViewModel>()
                       .ForMember(t => t.EmployeeName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "EmpName")))
                       .ForMember(t => t.BusinessEmployeeName, opt => opt.MapFrom(src => src.BusinessEmployee))
                       .ForMember(t => t.AreaName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Area")));

                    c.CreateMap<ReceiveReturnBusinessTripCardDataViewModel, ReceiveReturnBusinessTripCardDataView>();

                    CreateAcsMap(c);

                    CreateSecurityMap(c);

                    c.CreateMap<AcsTask, AcsTaskViewModel>();
                }
            );
        }

        private static void CreateAcsMap(IMapperConfigurationExpression c)
        {
            c.CreateMap<ReqApproverList, ReqApproverListViewModel>()
                .ForMember(s => s.RequestNo, opt => opt.MapFrom(s => s.ReqNo))
                .ForMember(s => s.RequestType, opt => opt.MapFrom(s => s.ReqType))
                .ForMember(s => s.Area, opt => opt.Condition(s => s.Area != null));

            c.CreateMap<ReqApproverListViewModel, ReqApproverList>()
                .ForMember(s => s.ReqNo, opt => opt.MapFrom(s => s.RequestNo))
                .ForMember(s => s.ReqType, opt => opt.MapFrom(s => s.RequestType))
                .ForMember(s => s.Area, opt => opt.Condition(s => s.Area != null));

            c.CreateMap<AcsEmployee, AcsEmployeeViewModel>()
                .ForMember(t => t.EntryTimeFrom, opt => opt.MapFrom(src => src.EntryTimeFrom.Hours * 60 + src.EntryTimeFrom.Minutes))
                .ForMember(t => t.EntryTimeTo, opt => opt.MapFrom(src => src.EntryTimeTo.Hours * 60 + src.EntryTimeTo.Minutes));


            c.CreateMap<AcsEmployeeViewModel, AcsEmployee>()
                .ForMember(t => t.EntryTimeFrom, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.EntryTimeFrom)))
                .ForMember(t => t.EntryTimeTo, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.EntryTimeTo)))
                .ForMember(t => t.AcsEmployeeDetails, opt => opt.Ignore());

            c.CreateMap<AcsEmployeeDetail, AcsEmployeeDetailViewModel>()
                .ForMember(t => t.EmployeeID, opt => opt.MapFrom(s => s.EmpID))
                .ForMember(t => t.EmployeeName, opt => opt.MapFrom(s => s.Employee != null ? ModelLocalizeManager.GetValue(s.Employee, "EmpName") : s.Name))
                .ForMember(t => t.DepartmentName, opt => opt.MapFrom(s => s.Employee != null ? ModelLocalizeManager.GetValue(s.Employee, "Department") : s.DeptName));

            c.CreateMap<AcsEmployeeImportData, AcsEmployeeDetailViewModel>();

            c.CreateMap<AcsPhoto, AcsPhotoViewModel>()
                .ForMember(t => t.EquipItem, opt => opt.MapFrom(src => src.Item))
                .ForMember(t => t.TakePhotoTimeFrom, opt => opt.MapFrom(src => src.TakePhotoTimeFrom.Hours * 60 + src.TakePhotoTimeFrom.Minutes))
                .ForMember(t => t.TakePhotoTimeTo, opt => opt.MapFrom(src => src.TakePhotoTimeTo.Hours* 60 + src.TakePhotoTimeTo.Minutes));
                
            c.CreateMap<AcsPhotoViewModel, AcsPhoto>()
                .ForMember(t => t.TakePhotoTimeFrom, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.TakePhotoTimeFrom)))
                .ForMember(t => t.TakePhotoTimeTo, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.TakePhotoTimeTo)));

            c.CreateMap<AcsVisitor, AcsVisitorViewModel>()
                .ForMember(t => t.EntryTimeFrom, opt => opt.MapFrom(src => src.EntryTimeFrom.Hours * 60 + src.EntryTimeFrom.Minutes))
                .ForMember(t => t.EntryTimeTo, opt => opt.MapFrom(src => src.EntryTimeTo.Hours * 60 + src.EntryTimeTo.Minutes));

            c.CreateMap<AcsVisitorViewModel, AcsVisitor>()
                .ForMember(t => t.EntryTimeFrom, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.EntryTimeFrom)))
                .ForMember(t => t.EntryTimeTo, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.EntryTimeTo)))
                .ForMember(t => t.AcsVisitorDetails, opt => opt.Ignore());

            c.CreateMap<AcsVisitorImportData, AcsVisitorDetailViewModel>()
                .ForMember(t => t.DepartmentName, opt => opt.MapFrom(s => s.Department));

            c.CreateMap<AcsVisitorDetailViewModel, AcsVisitorDetail>()
                .ForMember(t => t.DeptName, opt => opt.MapFrom(s => s.DepartmentName))
                .ForMember(t => t.Name, opt => opt.MapFrom(s => s.VisitorName));

            c.CreateMap<AcsVisitorDetail, AcsVisitorDetailViewModel>()
               .ForMember(t => t.DepartmentName, opt => opt.MapFrom(s => s.DeptName))
               .ForMember(t => t.VisitorName, opt => opt.MapFrom(s => s.Name));

            c.CreateMap<AcsVisitorTransactionDataView, AcsVisitorTransactionDataViewModel>()
             .ForMember(t => t.VerifyTypeName, opt => opt.MapFrom(s => ModelLocalizeManager.GetValue(s, "VerifyType")))
             .ForMember(t => t.DepartmentName, opt => opt.MapFrom(s => s.DeptName));

            c.CreateMap<TransactionAcs, AcsTransactionViewModel>();

            /* =========================================
             * AcsItemOut
             * =========================================*/
            c.CreateMap<AcsItemOut, AcsItemOutViewModel>()
                .ForMember(t => t.RequestNo, opt => opt.MapFrom(s => s.ReqNo))
                .ForMember(t => t.Department, opt => opt.MapFrom(s => s.DeptName));
                //.ForMember(t => t.AcsItemOutDetails, opt => opt.Ignore());

            c.CreateMap<AcsItemOutViewModel, AcsItemOut>()
                .ForMember(t => t.ReqNo, opt => opt.MapFrom(s => s.RequestNo))
                .ForMember(t => t.DeptName, opt => opt.MapFrom(s => s.Department))
                .ForMember(t => t.AcsItemOutDetail, opt => opt.Ignore());


            /* =========================================
             * AcsItemIn
             * =========================================*/
            c.CreateMap<AcsItemIn, AcsItemInViewModel>()
                .ForMember(t => t.AcknowledgeDate, opt => opt.MapFrom(s => s.AckDate));               

            c.CreateMap<AcsItemInViewModel, AcsItemIn>()
                .ForMember(t => t.AckDate, opt => opt.MapFrom(s => s.AcknowledgeDate))
                .ForMember(t => t.AcsItemInDetails, opt => opt.Ignore());

            c.CreateMap<AcsItemInDetailDataView, AcsItemInDetailViewModel>()
                .ForMember(t => t.ActualReturnQty, opt => opt.MapFrom(s => s.ActReturnQty))
                .ForMember(t => t.ActualTakeInQty, opt => opt.MapFrom(s => s.ActInQty))
                .ForMember(t => t.RequestItemQty, opt => opt.MapFrom(s => s.ReqQty))
                .ForMember(t => t.ActualReturnDate, opt => opt.MapFrom(s => s.ActReturnDate));

            c.CreateMap<AcsItemInDetail, AcsItemInDetailViewModel>()
                .ForMember(t => t.ActualReturnDate, opt => opt.MapFrom(s => s.ActReturnDate))
                .ForMember(t => t.RequestItemQty, opt => opt.MapFrom(s => s.ReqQty))
                .ForMember(t => t.ActualTakeInQty, opt => opt.MapFrom(s => s.ActInQty))
                .ForMember(t => t.ActualReturnQty, opt => opt.MapFrom(s => s.ActReturnQty));
         

        }

        private static void CreateSecurityMap(IMapperConfigurationExpression c)
        {
            c.CreateMap<User, AccountViewModel>()
                .ForMember(t => t.EmployeeID, opt => opt.MapFrom(src => src.EmpID))
                .ForMember(t => t.Roles, opt => opt.MapFrom(src => src.Roles.Select(t => t.Name)))
                .ForMember(t => t.RegisteredDateTime, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(t => t.LastSignedInDateTime, opt => opt.MapFrom(src => src.LastSignedInDate));

            c.CreateMap<AccountViewModel, User>();
            c.CreateMap<Role, RoleViewModel>();
            c.CreateMap<RoleViewModel, Role>();
            c.CreateMap<Role, UserRole>();

            c.CreateMap<EmployeeInformation, EmployeeInformationViewModel>()
                .ForMember(t => t.EmployeeID, opt => opt.MapFrom(src => src.EmpID))
                .ForMember(t => t.EmployeeName, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "EmpName")))
                .ForMember(t => t.Position , opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Position")))
                .ForMember(t => t.Department, opt => opt.MapFrom(src => ModelLocalizeManager.GetValue(src, "Department")));
        }
    }
}