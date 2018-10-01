using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SECOM.ACS.Web.Mvc
{
    public class ApplicationHandleErrorAttribute : HandleErrorAttribute
    {
        public string LoggingName { get; set; } = "application";

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                var logger = LogManager.GetLogger(this.LoggingName);              
                logger.Error(new { errorMessage = filterContext.Exception.Message, stackTrace=filterContext.Exception.StackTrace } );
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.ExceptionHandled = true;
                }
            }
            base.OnException(filterContext);
        }
    }
}
