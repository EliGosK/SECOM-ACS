using CSI.ComponentModel;
using CSI.Localization;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Html.Models;
using Resources;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RS = SECOM.ACS.MvcWebApp.Resources;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class AcsModelHelper
    {
        public static NameValueOptions GetMiscStatus()
        {
            var dataItems = new NameValueOptions() {
                new NameValueOption(RS.ModelMetadataResource.MiscStatus_Active,  MiscStatus.Active),
                new NameValueOption( RS.ModelMetadataResource.MiscStatus_Inactive, MiscStatus.Inactive)
            };
            return dataItems;
        }

        public static NameValueOptions GetAreaStatus()
        {
            var dataItems = new NameValueOptions() {
                new NameValueOption(RS.ModelMetadataResource.AreaStatus_Active,  AreaStatus.Active),
                new NameValueOption( RS.ModelMetadataResource.AreaStatus_Inactive, AreaStatus.Inactive)
            };
            return dataItems;
        }

        public static NameValueOptions GetItemStatus()
        {
            var dataItems = new NameValueOptions() {
                new NameValueOption( RS.ModelMetadataResource.ItemStatus_Active, ItemStatus.Active ),
                new NameValueOption(RS.ModelMetadataResource.ItemStatus_Inactive,ItemStatus.Inactive)
            };
            return dataItems;
        }

        public static NameValueOptions GetCardStatus()
        {
            var dataItems = new NameValueOptions() {
                new NameValueOption(RS.ModelMetadataResource.CardStatus_Active, CardStatus.Active ),
                new NameValueOption(RS.ModelMetadataResource.CardStatus_Inactive, CardStatus.Inactive )
            };
            return dataItems;
        }

        public static NameValueOptions GetPhotoByTypes()
        {
            var dataItems = new NameValueOptions() {
                new NameValueOption(RS.ModelMetadataResource.PhotoByType_Employee, PhotoByTypes.Employee ),
                new NameValueOption(RS.ModelMetadataResource.PhotoByType_Outsource, PhotoByTypes.OutSource )
            };
            return dataItems;
        }

        public static string GenerateEmptyRequestNo(string prefix)
        {
            return String.Format("{0}{1:yyyyMMdd}-{2:000}", prefix, DateTime.Now, 0);
        }

        public static string GetRequestStatusName(string status)
        {
            var findStatus = ApplicationContext.DataContext.SystemMiscs.FirstOrDefault(t => t.SysMiscType == "Status" && t.SysMiscCode == status);
            if (findStatus == null) { return status; }
            return ModelLocalizeManager.GetValue(findStatus, "SysMisc");
        }

        public static object GetRegisterCardTypes()
        {
            return ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.RegisterCardType)
                .Select(t => new { Name = t.SysMiscValue1, Value = t.SysMiscCode })
                .ToList();
        }

        public static DateTime GetFirstDateOfMonth(int monthOffset = 0)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthOffset);
        }

        public static DateTime GetLastDateOfMonth(int monthOffset = 0)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthOffset + 1).AddDays(-1);
        }

        public static double GetMegaByte(int value)
        {
            return value * 1024 * 1024;
        }

        public static string DisplayFileSize(long fileSize)
        {
            var units = new string[] { "Bytes", "KB", "MB", "GB", "TB" };
            double temp = fileSize;
            foreach (var unit in units)
            {
                if (temp < 1024) { return String.Format("{0:N2} {1}", temp, unit); }
                temp = temp / 1024;
            }
            return String.Format("{0:N2}", fileSize);
        }

        public static Dictionary<string,string> GetModelDisplayNames<TModel>() where TModel : class
        {
            var values = new Dictionary<string, string>();
            var properties = typeof(TModel).GetProperties();
            foreach (var p in properties)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null,typeof(TModel),p.Name);
                string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(p.Name));
                values.Add(p.Name, value);
            }
            return values;
        }

        public static Dictionary<string, string> GetModelDisplayNames(Type type)
        {
            var values = new Dictionary<string, string>();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null, type, p.Name);
                string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(p.Name));
                values.Add(p.Name, value);
            }
            return values;
        }

        public static string GetFactory(string code)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.Factory && t.SysMiscCode == code).FirstOrDefault();
            return dataItem != null ? dataItem.SysMiscValue3 : String.Empty;
        }

        public static string GetLocalizeSiteMapNodeTitle(SiteMapNodeModel node)
        {
            if (MvcSiteMapProvider.SiteMaps.Current.EnableLocalization)
            {
                var objectId = node.Attributes.ContainsKey("objectId")? (string)node.Attributes["objectId"] : "";
                if (String.IsNullOrEmpty(objectId)) { return node.Title; }
                return (string)HttpContext.GetGlobalResourceObject("SiteMapLocalizationResource", objectId);
            }
            return node.Title;
        }

        public static ISiteMapNode FindSiteMapNodeByObjectId(string objectId)
        {
            return FindSiteMapNodeByObjectId(objectId,MvcSiteMapProvider.SiteMaps.Current.RootNode);
        }

        private static ISiteMapNode FindSiteMapNodeByObjectId(string objectId,ISiteMapNode node)
        {
            if (node.Attributes.ContainsKey("objectId"))
            {
                var id =  (string)node.Attributes["objectId"];
                if (objectId == id) { return node; }
            }

            foreach (var childNode in node.ChildNodes)
            {
                var findNode = FindSiteMapNodeByObjectId(objectId, childNode);
                if (findNode != null) { return findNode; }
            }

            return null;
        }

        public static string GetGridDateFormat()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            return String.Concat("{0:", culture.DateTimeFormat.ShortDatePattern,"}");
        }

        public static string GetGridDateTimeFormat()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            return String.Concat("{0:", culture.DateTimeFormat.LongDatePattern, "}");
        }

        public static string GetGridTimeFormat()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            return String.Concat("{0:", culture.DateTimeFormat.ShortTimePattern, "}");
        }

        public static string GetDateFormat()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            return String.Concat("dd/MM/yyyy");
        }

        public static bool IsRequiredSuperiorApproval(string positionID)
        {
            var positions = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.PositionLevel && t.SysMiscSortNo <= 3)
                .Select(t=>t.SysMiscValue1).ToList();

            return !positions.Contains(positionID);
        }
    }
}