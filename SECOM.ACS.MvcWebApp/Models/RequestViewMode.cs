using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public enum RequestViewMode
    {
        View,
        Approval
    }

    public class DashboardDataSourceResult : DataSourceResult
    {
        public string ViewName { get; set; }

        public static DashboardDataSourceResult ToViewResult(string viewName, DataSourceResult result)
        {
            return new DashboardDataSourceResult()
            {
                AggregateResults = result.AggregateResults,
                Data = result.Data,
                Errors = result.Errors,
                Total = result.Total,
                ViewName = viewName
            };
        }
    }
    
}