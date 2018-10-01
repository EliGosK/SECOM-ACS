using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.MvcWebApp
{
    public static class MessageHelper
    {
        public static string DataNotFound(string name)
        {
            return String.Format(WidgetResource.DataNotFound, name);
        }

        public static string CreateSuccess(string name)
        {
            return String.Format(WidgetResource.CreateObjectSuccess, name);
        }

        public static string CreateFailed(string name, string errorMessage = null)
        {
            var message = String.Format(WidgetResource.CreateObjectFailed, name);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                message = string.Concat(message, " ", errorMessage);
            }
            return message;
        }

        public static string GenerateReportSuccess(string name)
        {
            return String.Format(WidgetResource.GenerateReportSuccess, name);
        }

        public static string UpdateObjectSuccess(string name)
        {
            return String.Format(WidgetResource.UpdateObjectSuccess, name);
        }

        public static string UpdateObjectFailed(string name, string errorMessage = null)
        {
            var message = String.Format(WidgetResource.UpdateObjectFailed, name);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                message = string.Concat(message, " ", errorMessage);
            }
            return message;
        }

        public static string DeleteObjectSuccess(string name)
        {
            return String.Format(WidgetResource.DeleteObjectSuccess, name);
        }

        public static string DeleteObjectFailed(string name, string errorMessage = null)
        {
            var message = String.Format(WidgetResource.DeleteObjectFailed, name);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                message = string.Concat(message, " ", errorMessage);
            }
            return message;
        }
    }
}
