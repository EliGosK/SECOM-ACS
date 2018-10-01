using CSI.Core;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CSI.Web.Mvc.KendoUI.Extensions
{
    public static class KendoExtensions
    {
        public static DataSourceResult ToDataSourceResult<T>(this IPagedList<T> list)
           where T : class
        {
            return new DataSourceResult()
            {
                Data = list,
                Total = list.TotalRecords
            };
        }

        public static DataSourceResult ToDataSourceResult<T>(this IPagedList<T> list,int totalRecords)
          where T : class
        {
            return new DataSourceResult()
            {
                Data = list,
                Total = totalRecords,               
            };
        }

        public static DataSourceResult ToDataSourceResult<T,TResult>(this IPagedList<T> list,Func<T,TResult> selector) 
            where T : class
        {
            var dataList = new List<TResult>();
            foreach (var item in list)
            {
                dataList.Add(selector.Invoke(item));
            }

            return new DataSourceResult()
            {
                Data = dataList,
                Total = list.TotalRecords
            };
        }

        public static MvcHtmlString KendoActionLink(this HtmlHelper helper, string name, string icon, string text,string url, object htmlAttributes = null)
        {
            var attrs = HtmlHelper.ObjectToDictionary(htmlAttributes?? new { });
            if (!attrs.ContainsKey("href"))
            {
                attrs.Add("href", url);
            }
            string content = String.Format("<i class='fa fa-{0}'></i> {1}", icon, text);
            var html = helper.Kendo().Button().Name(name).Tag("a").Content(content).HtmlAttributes(attrs).ToHtmlString();
            return MvcHtmlString.Create(html);
        }


        public static MvcHtmlString KendoConfirmButton(this HtmlHelper helper, string name, string icon, string text, object htmlAttributes = null)
        {
            string content = String.Format("<i class='fa fa-{0}'></i> {1}", icon, text);
            return helper.KendoConfirmButton(name, content, htmlAttributes);
        }

        public static MvcHtmlString KendoConfirmButton(this HtmlHelper helper, string name,string content, object htmlAttributes = null)
        {
            htmlAttributes = htmlAttributes ?? new { };
            var defaultDataAttrs = new
            {
                @data_role = "confirm",
                @data_title = "Confirmation",
                @data_content = "Are you sure to continue?",
                @data_icon = "fa fa-question-circle",
                @data_use_bootstrap = "false",
                @data_escape_key = "true",
                @data_theme = "dark"
            };
            var dataAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(defaultDataAttrs);
            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            foreach (var key in dataAttrs.Keys)
            {
                if (!attrs.ContainsKey(key))
                {
                    attrs.Add(key, dataAttrs[key]);
                }
            }

            string html = helper.Kendo().Button()
                            .Name(name)
                            .Content(content)
                            .HtmlAttributes(attrs)                            
                            .ToHtmlString();
            return MvcHtmlString.Create(html);
        }

        public static void ApplyDataSourceRequest(this ISupportPagingOptions options, DataSourceRequest request)
        {
            options.PageIndex = request.Page;
            options.PageSize = request.PageSize;
        }

        public static MvcHtmlString KendoValidationMessage(this HtmlHelper helper, string name)
        {
            TagBuilder builder = new TagBuilder("span");
            builder.MergeAttribute("data-for", name);
            builder.MergeAttribute("class", "k-invalid-msg");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString KendoValidationMessageFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            TagBuilder builder = new TagBuilder("span");
            builder.MergeAttribute("data-for", metadata.PropertyName);
            builder.MergeAttribute("class", "k-invalid-msg");

            //var message = helper.ValidationMessageFor<TModel, TProperty>(expression);
            //if (message != null)
            //{
            //    return MvcHtmlString.Create(String.Concat(message.ToHtmlString(), builder.ToString()));
            //}
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString KendoValidationSummary(this HtmlHelper helper, string id = "validation-summary", object htmlAttributes = null)
        {
            var attrs = HtmlHelper.ObjectToDictionary(htmlAttributes);
            TagBuilder builder = new TagBuilder("div");
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("class", "alert alert-danger alert-block k-validation-summary");
            builder.MergeAttribute("style", "display:none");
            foreach (var attr in attrs)
            {
                if (builder.Attributes.ContainsKey(attr.Key))
                {
                    builder.Attributes[attr.Key] = String.Concat(builder.Attributes[attr.Key], ";", (string)attr.Value);
                }
                else {
                    builder.Attributes.Add(attr.Key, (string)attr.Value);
                }
            }
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}
