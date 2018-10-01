using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CSI.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString AssemblyVersion(this HtmlHelper helper, Type applicationType,string format = "{0}")
        {
            var version = applicationType.Assembly.GetName().Version;
            TagBuilder builder = new TagBuilder("span");
            builder.InnerHtml = String.Format(format, version);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString FileLastWriteTime(this HtmlHelper helper, string file, string format = "{0:yyyy.MM.dd.HHmmss}", string culture = "th-TH")
        {
            TagBuilder builder = new TagBuilder("span");
            if (File.Exists(file))
            {
                builder.InnerHtml = String.Format(CultureInfo.CreateSpecificCulture(culture), format, File.GetLastWriteTime(file));
            }
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));

        }

        public static MvcHtmlString LabelRequiredHintFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string hintCssClass = "required-hint")
        {
            return helper.LabelRequiredHintFor(expression, null, hintCssClass);
        }

        public static MvcHtmlString LabelRequiredHintFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, string hintCssClass = "required-hint")
        {
            var attrs = HtmlHelper.ObjectToDictionary(htmlAttributes ?? new { });
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var labelText = metadata.DisplayName ?? metadata.PropertyName;
            if (metadata.IsRequired)
            {
                if (attrs.ContainsKey("class"))
                {
                    var css = attrs["class"] as string;
                    if (!css.Split(new char[] { ' ' }).Where(t => t == hintCssClass).Any())
                    {
                        attrs["class"] = attrs["class"] + " " + hintCssClass;
                    }
                }
                else
                {
                    attrs.Add("class", hintCssClass);
                }
            }
            return helper.LabelFor<TModel, TProperty>(expression, labelText, attrs);
        }

        public static IHtmlString DisplayHtmlFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, string>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            Object value = metadata.Model;

            var html = System.Web.HttpUtility.HtmlDecode((string)value);
            return  helper.Raw(html);
        }

        public static IHtmlString DisplayHtml<TModel>(this HtmlHelper<TModel> helper, string value)
        {
            var html = System.Web.HttpUtility.HtmlDecode(value);
            return helper.Raw(html);
        }

        /// <summary>
        ///     Compares the requested route with the given <paramref name="value" /> value, if a match is found the
        ///     <paramref name="attribute" /> value is returned.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">The action value to compare to the requested route action.</param>
        /// <param name="attribute">The attribute value to return in the current action matches the given action value.</param>
        /// <returns>A HtmlString containing the given attribute value; otherwise an empty string.</returns>
        public static IHtmlString RouteIf(this HtmlHelper helper, string value, string attribute)
        {
            var currentAction =
                (helper.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();

            var hasAction = value.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase);

            return hasAction ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }

        public static IHtmlString RouteIf(this HtmlHelper helper, string controllerName, string actonName, string attribute)
        {
            var currentController =
                (helper.ViewContext.RequestContext.RouteData.Values["controller"] ?? string.Empty).ToString().UnDash();
            var currentAction =
                (helper.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();

            var hasController = controllerName.Equals(currentController, StringComparison.InvariantCultureIgnoreCase);
            var hasAction = actonName.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase);

            return hasAction && hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }

        /// <summary>
        ///     Renders the specified partial view with the parent's view data and model if the given setting entry is found and
        ///     represents the equivalent of true.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="condition">The boolean value that determines if the partial view should be rendered.</param>
        public static void RenderPartialIf(this HtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
                return;

            htmlHelper.RenderPartial(partialViewName);
        }

        /// <summary>
        ///     Returns an unordered list (ul element) of validation messages that utilizes bootstrap markup and styling.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="alertType">The alert type styling rule to apply to the summary element.</param>
        /// <param name="heading">The optional value for the heading of the summary element.</param>
        /// <returns></returns>
        public static HtmlString ValidationBootstrap(this HtmlHelper htmlHelper, string alertType = "danger",
            string heading = "")
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
                return new HtmlString(string.Empty);

            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"alert alert-{0} alert-block\">", alertType);
            sb.Append("<button class=\"close\" data-dismiss=\"alert\" aria-hidden=\"true\">&times;</button>");

            if (!String.IsNullOrWhiteSpace(heading))
            {
                sb.AppendFormat("<h4 class=\"alert-heading\">{0}</h4>", heading);
            }

            sb.Append(htmlHelper.ValidationSummary());
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ConfirmButton(this HtmlHelper helper, string name, string content, object htmlAttributes = null)
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

            var buttonBuilder = new TagBuilder("button");
            buttonBuilder.Attributes.Add("id", name);
            buttonBuilder.Attributes.Add("name", name);
            buttonBuilder.InnerHtml = content;
            foreach (var attr in attrs)
            {
                buttonBuilder.Attributes.Add(attr.Key, (string)attr.Value);
            }
            return MvcHtmlString.Create(buttonBuilder.ToString());
        }

        public static MvcHtmlString DisplayName<TModel>(this HtmlHelper helper, string propertyName)
        {
            var value = ModelMetadataUtility.GetModelDisplayName(typeof(TModel), propertyName);
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => Activator.CreateInstance<TModel>(), typeof(TModel), propertyName);
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString DisplayName<TModel>(this HtmlHelper helper, Expression<Func<TModel, string>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var value = ModelMetadataUtility.GetModelDisplayName(typeof(TModel), propertyName);
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString DisplayName<TModel, TProperty>(this HtmlHelper helper, Expression<Func<TModel, TProperty>> expression)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var value = ModelMetadataUtility.GetModelDisplayName(typeof(TModel), propertyName);
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString LabelDisplayName<TModel>(this HtmlHelper html, string propertyName, object htmlAttributes) where TModel : class
        {
            return html.Label(ModelMetadataUtility.GetModelDisplayName<TModel>(propertyName), htmlAttributes);
        }

        public static MvcHtmlString LabelDisplayName<TModel>(this HtmlHelper html, Expression<Func<TModel,string>> expression, object htmlAttributes) where TModel : class
        {
            //var value = ModelMetadataUtility.GetModelDisplayName<TModel>(ExpressionHelper.GetExpressionText(expression));
            return html.Label(ExpressionHelper.GetExpressionText(expression), htmlAttributes);
        }
    }
}
