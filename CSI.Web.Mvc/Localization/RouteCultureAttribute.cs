using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    public class RouteCultureAttribute : AuthorizeAttribute
    {
        private readonly string parameterName;
        private readonly string cookiesName;

        public RouteCultureAttribute(string name = "culture", string cookiesName = "locale")
        {
            this.parameterName = name;
            this.cookiesName = cookiesName;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var values = filterContext.RouteData.Values;
            string culture = (string)values[parameterName] ?? "";
            if (String.IsNullOrEmpty(culture))
            {
                var cookieLocale = filterContext.HttpContext.Request.Cookies[cookiesName];
                if (cookieLocale != null)
                {
                    culture = cookieLocale.Value;
                }
            }

            if (!String.IsNullOrEmpty(culture))
            {
                CultureInfo ci = new CultureInfo(culture);
                //Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
        }
    }
}
