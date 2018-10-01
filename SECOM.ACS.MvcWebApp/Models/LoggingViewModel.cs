using CSI.ComponentModel;
using CSI.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class LoggingViewModel
    {
        public string LoggingFileName { get; set; }
        public string[] Messages { get; set; }
        public long FileSize { get; set; }
        public DateTime LoggingDateTime { get; set; }
    }

    public class LoggingErrorViewModel
    {
        [JsonProperty("errorMessage")]
        public string Message { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }
    }

    public class LoggingSearchCriteria : PagingOptions
    {
        public string Logger { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }


    public class ApplicationLogType
    {
        public const string Workflow = "workflow";
        public const string Applicaiton = "app";
    }
}