using CSI.Core.Extensions;
using CSI.Localization;
using SECOM.ACS.Identity;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.Linq;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static partial class ModelExtensions
    {
        public static RequestInquriyDataViewModel ToViewModel(this RequestDataView model)
        {
            return new RequestInquriyDataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateFrom = model.EntryDateFrom,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH01DataViewModel ToViewModel(this RequestDH01DataView model)
        {
            return new RequestDH01DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateFrom = model.EntryDateFrom,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH02DataViewModel ToViewModel(this RequestDH02DataView model)
        {
            return new RequestDH02DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateFrom = model.EntryDateFrom,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH03DataViewModel ToViewModel(this RequestDH03DataView model)
        {
            return new RequestDH03DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateFrom = model.EntryDateFrom,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH04DataViewModel ToViewModel(this RequestDH04DataView model)
        {
            return new RequestDH04DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateFrom = model.EntryDateFrom,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH05DataViewModel ToViewModel(this RequestDH05DataView model)
        {
            return new RequestDH05DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                TakeOutDate = model.TakeOutDate,
                TakeOutCompany = model.TakeOutCompany,
                TakeOutDepartment = model.TakeOutDepartment,
                TakeOutName = model.TakeOutName,
                SourcePlace = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntrTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH06DataViewModel ToViewModel(this RequestDH06DataView model)
        {
            return new RequestDH06DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                TakeInDate = model.TakeInDate,
                TakeOutCompany = model.TakeOutCompany,
                TakeOutDepartment = model.TakeOutDepartment,
                TakeInName = model.TakeInName,
                BringingArea = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntrTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH07DataViewModel ToViewModel(this RequestDH07DataView model)
        {
            return new RequestDH07DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                EntryDateFrom = model.EntryDateFrom,
                EquipName = ModelLocalizeManager.GetValue(model, "EquipName"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                AssetCode = model.AssetCode,
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }

        public static RequestDH08DataViewModel ToViewModel(this RequestDH08DataView model)
        {
            return new RequestDH08DataViewModel
            {
                ObjectID = model.ObjectID,
                DocumentType = ModelLocalizeManager.GetValue(model, "DocumentType"),
                EntryDateFrom = model.EntryDateFrom,
                EquipName = ModelLocalizeManager.GetValue(model, "EquipName"),
                Area = ModelLocalizeManager.GetValue(model, "Area"),
                EntryDateTo = model.EntryDateTo,
                EntryTimeFrom = model.EntryTimeFrom,
                EntryTimeTo = model.EntryTimeTo,
                ReqStatus = model.ReqStatus,
                ReqStatusName = ModelLocalizeManager.GetValue(model, "ReqStatus"),
                ReqNo = model.ReqNo,
                RequestBy = ModelLocalizeManager.GetValue(model, "RequestBy"),
                RequestDate = model.RequestDate
            };
        }
    }
}