using SECOM.ACS.MvcWebApp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp
{
    public class SiteMapPageTitleAttribute : ActionFilterAttribute
    {
        public string ObjectId { get; private set; }

        public SiteMapPageTitleAttribute(string objectId)
            : base()
        {
            this.ObjectId = objectId;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var node = AcsModelHelper.FindSiteMapNodeByObjectId(this.ObjectId);
            if (node != null)
            {
                filterContext.Controller.ViewBag.PageTitle = (string)HttpContext.GetGlobalResourceObject("SiteMapLocalizationResource", this.ObjectId);
                filterContext.Controller.ViewBag.PageIcon = node.Attributes.ContainsKey("icon") ? "fa fa-" + (string)node.Attributes["icon"] : "";
            }
        }
    }
}