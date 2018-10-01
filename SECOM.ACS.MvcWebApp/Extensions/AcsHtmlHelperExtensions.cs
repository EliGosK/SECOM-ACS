using CSI.IO;
using CSI.Localization;
using CSI.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using RS = SECOM.ACS.MvcWebApp.Resources;
using WH = System.Web.Helpers;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class AcsHtmlHelperExtensions
    {
        public static ButtonBuilder SearchButton(this HtmlHelper helper, string name = "search-button", object htmlAttributes = null)
        {
            return helper.CommandButton(name, "search", RS.WidgetResource.Button_Search_Text, htmlAttributes);
        }

        public static ButtonBuilder ClearButton(this HtmlHelper helper, string name = "clear-button", object htmlAttributes = null)
        {
            return helper.CommandButton(name, "times-circle", RS.WidgetResource.Button_Clear_Text, htmlAttributes);
        }

        public static ButtonBuilder ResetButton(this HtmlHelper helper, string name = "reset-button", object htmlAttributes = null)
        {
            return helper.CommandButton(name, "times-circle", RS.WidgetResource.Button_Reset_Text, htmlAttributes);
        }

        public static ButtonBuilder RetrieveButton(this HtmlHelper helper, string name= "retrieve-button", object htmlAttribute = null)
        {
            return helper.CommandButton(name, "search", RS.WidgetResource.Button_Retrieve_Text, htmlAttribute);
        }

        public static ButtonBuilder RetrieveButton2(this HtmlHelper helper, string name = "retrieve-button2", object htmlAttribute = null)
        {
            return helper.CommandButton(name, "search", RS.WidgetResource.Button_Retrieve_Text, htmlAttribute);
        }

        public static ButtonBuilder RetrieveClearButton(this HtmlHelper helper, string name = "retrieve-clear-button", object htmlAttributes = null)
        {
            return helper.CommandButton(name, "times-circle", RS.WidgetResource.Button_Clear_Text, htmlAttributes);
        }

        public static ButtonBuilder CommandButton(this HtmlHelper helper, string name, string icon, string text, object htmlAttributes = null)
        {
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { });
            icon = String.IsNullOrEmpty(icon) ? " " : String.Format("<i class=\"fa fa-{0}\"></i>", icon);
            return helper.Kendo().Button().Name(name)
                .Content(icon + " " + text)
                .HtmlAttributes(attrs);
        }

        public static MvcHtmlString GridAddButton(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<a class=\"k-button k-primary k-grid-add\"><i class=\"fa fa-plus-circle\"></i> " + RS.WidgetResource.Button_Add_Text + "</a>");
        }

        public static MvcHtmlString GridEditButton(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<a class=\"k-button k-button-sm k-primary k-grid-edit\"><span class=\"k-icon k-i-edit\" title=" + RS.WidgetResource.Button_Edit_Title + "></span></a>");
        }

        public static MvcHtmlString GridDeleteButton(this HtmlHelper helper)
        {
            return MvcHtmlString.Create("<a class=\"k-button k-button-sm k-button-danger k-grid-delete\" title=" + RS.WidgetResource.Button_Delete_Title + "><span class=\"k-icon k-i-close\"></span></a>");
        }

        public static MvcHtmlString GridEditDeleteButtons(this HtmlHelper helper)
        {
            var html = String.Format("{0} {1}", helper.GridEditButton().ToHtmlString(), helper.GridDeleteButton().ToHtmlString());
            return MvcHtmlString.Create(html);
        }

        public static IHtmlString JsonSystemMisc(this HtmlHelper helper, string type = null)
        {
            var miscs = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == type || String.IsNullOrEmpty(type))
                .Select(t => new { MiscType = t.SysMiscType, Value = t.SysMiscCode, Name = ModelLocalizeManager.GetValue(t, "SysMisc") })
                .ToList();
            return helper.Raw(WH.Json.Encode(miscs));
        }


        public static DropDownListBuilder DropDownTimePickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, TimeSpan startTime,TimeSpan endTime,int internval = 60)
        {
            var values = new List<Tuple<string, int>>();
            var t = startTime;
            while (t.Ticks <= endTime.Ticks)
            {
                values.Add(new Tuple<string, int>(String.Format("{0}:{1:00}",t.Hours, t.Minutes), Convert.ToInt32( t.TotalMinutes)));
                t = t.Add(TimeSpan.FromMinutes(internval));
            }
            var t2 = new TimeSpan(23, 59,0);
            values.Add(new Tuple<string, int>(String.Format("{0}:{1:00}", t2.Hours, t2.Minutes), Convert.ToInt32(t2.TotalMinutes)));
            return helper.Kendo().DropDownListFor(expression).BindTo(values)
                .DataTextField("Item1")
                .DataValueField("Item2");
        }

        public static MvcHtmlString DisplayLocalize<TModel>(this HtmlHelper helper, TModel model, string name)
        {
            if (model == null) { return MvcHtmlString.Create(""); }
            return MvcHtmlString.Create(ModelLocalizeManager.GetValue(model, name));
        }

        public static MvcHtmlString DisplayUploadHint(this HtmlHelper helper, string[] allowedFileExtensions, long maxFileSize)
        {
            var extensions = "*.*";
            if (allowedFileExtensions != null && allowedFileExtensions.Length > 0)
            {
                extensions = String.Join(", ", allowedFileExtensions);
            }
            var sb = new StringBuilder();
            sb.Append("<div class='upload-hint'><i class='fa fa-exclamation-circle'></i> ");
            sb.AppendFormat(ViewResource.UploadHint, extensions, AcsModelHelper.DisplayFileSize(maxFileSize));
            sb.Append("</div>");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DisplayDate(this HtmlHelper helper, DateTime? date, string format = "", string culture = "")
        {
            var c = String.IsNullOrEmpty(culture) ? System.Threading.Thread.CurrentThread.CurrentUICulture : CultureInfo.GetCultureInfo(culture);
            return MvcHtmlString.Create(date.HasValue ? (String.IsNullOrEmpty(format) ? date.Value.ToString(c) : date.Value.ToString(format, c)) : "");
        }

        public static MvcHtmlString UniteGalleryFromJsonFile(this HtmlHelper helper, string name, string jsonFile)
        {
            var t = new TagBuilder("div");
            t.Attributes.Add("id", name);
            t.MergeAttribute("style", "display:none");
            var dataItems = JsonDataContext.GetDataFromJsonFile<UniteGalleryData>(jsonFile);
            var sb = new StringBuilder();
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var dataItem in dataItems)
            {
                var img = new TagBuilder("img");
                img.Attributes.Add("alt", dataItem.AlternativeText ?? "");
                img.Attributes.Add("src", urlHelper.Content(dataItem.ThumbnailImageUrl ?? dataItem.ImageUrl));
                img.Attributes.Add("data-image", urlHelper.Content(dataItem.ImageUrl));
                img.Attributes.Add("data-description", dataItem.Description ?? "");
                sb.AppendLine(img.ToString());
            }
            t.InnerHtml = sb.ToString();
            return MvcHtmlString.Create(t.ToString());
        }

        public static MvcHtmlString UniteGallery(this HtmlHelper helper, string name, string folder)
        {
            return helper.UniteGallery(name, folder, new string[] { "gif", "jpg", "tiff", "png" });
        }

        public static MvcHtmlString UniteGallery(this HtmlHelper helper, string name, string folder, string[] filters)
        {
            var t = new TagBuilder("div");
            t.Attributes.Add("id", name);
            t.MergeAttribute("style", "display:none");
            var sb = new StringBuilder();
            var appPath = PathUtility.GetPhysicalPath("~/");
            folder = PathUtility.GetPhysicalPath(folder);
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var file in DirectoryHelper.GetFiles(folder, filters, false))
            {
                var fileName = System.IO.Path.GetFileName(file);
                var desc = System.IO.Path.GetFileNameWithoutExtension(file);
                var imageUrl = file.Replace(appPath, "~/").Replace("\\", "/");
                var img = new TagBuilder("img");
                img.Attributes.Add("alt", fileName);
                img.Attributes.Add("src", urlHelper.Content(imageUrl));
                img.Attributes.Add("data-image", urlHelper.Content(imageUrl));
                img.Attributes.Add("data-description", desc);
                sb.AppendLine(img.ToString());
            }
            t.InnerHtml = sb.ToString();
            return MvcHtmlString.Create(t.ToString());
        }


        public static MvcHtmlString DisplayPhotoType(this HtmlHelper helper, string value)
        {
            var dataItem = AcsModelHelper.GetPhotoByTypes().Find(t => t.Value == value);
            if (dataItem != null)
            {
                return MvcHtmlString.Create(dataItem.Name);
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString DisplayFactory(this HtmlHelper helper, string code)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.Factory && t.SysMiscCode == code).FirstOrDefault();
            if (dataItem != null)
            {
                return MvcHtmlString.Create(dataItem.SysMiscValue3);
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString DisplayRequestStatusName2(this HtmlHelper helper, string status)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.Status && t.SysMiscCode == status).FirstOrDefault();
            if (dataItem != null)
            {
                return MvcHtmlString.Create(ModelLocalizeManager.GetValue(dataItem, "SysMisc"));
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString DisplayRequestStatusName(this HtmlHelper helper, string status)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == SystemMiscTypes.Status && t.SysMiscCode == status).FirstOrDefault();

            var builder = new TagBuilder("label");
            builder.AddCssClass("label label-md");
            if (dataItem != null)
            {
                switch (status)
                {
                    case RequestStatus.Cancel:
                        builder.AddCssClass("label-warning"); break;
                    case RequestStatus.Rejected:                    
                        builder.AddCssClass("label-danger"); break;
                    case RequestStatus.Approving:
                        builder.AddCssClass("label-primary"); break;
                    case RequestStatus.Approved:
                        builder.AddCssClass("label-success"); break;
                    case RequestStatus.Done:
                    case RequestStatus.Expired:
                        builder.AddCssClass("label-default"); break;
                    default:
                        builder.AddCssClass("label-primary"); break;
                }
                builder.SetInnerText(ModelLocalizeManager.GetValue(dataItem, "SysMisc"));
            }
            else
            {
                builder.SetInnerText(status);
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString DisplayRequestFor(this HtmlHelper helper, string requestFor)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.FirstOrDefault(t => t.SysMiscType == SystemMiscTypes.RequestFor && t.SysMiscCode == requestFor);
            if (dataItem != null)
            {
                return MvcHtmlString.Create(ModelLocalizeManager.GetValue(dataItem, "SysMisc"));
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString DisplayApprovalCode(this HtmlHelper helper,string approvalCode)
        {
            if (String.IsNullOrEmpty(approvalCode)) { return MvcHtmlString.Create(ModelMetadataResource.ApprovalCode_WaitApproval); }
            var dataItem = ApplicationContext.DataContext.SystemMiscs.FirstOrDefault(t => t.SysMiscType == SystemMiscTypes.ApprovalCode && t.SysMiscCode == approvalCode);
            if (dataItem != null)
            {
                return MvcHtmlString.Create(ModelLocalizeManager.GetValue(dataItem, "SysMisc"));
            }
            return MvcHtmlString.Empty;
        }

       
    }

    
}