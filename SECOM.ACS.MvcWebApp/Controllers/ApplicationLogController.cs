using CSI.Core;
using CSI.IO;
using CSI.Web.Mvc.KendoUI.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class ApplicationLogController : AppControllerBase
    {
        [ApplicationAuthorize("SYS060",PermissionNames.View)]
        [SiteMapPageTitle("SYS060")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListLogging([DataSourceRequest] DataSourceRequest request, LoggingSearchCriteria criteria)
        {
            criteria.ApplyDataSourceRequest(request);
            var folder = FileHelper.GetApplicationFullPath(ApplicationContext.Setting.ApplicationLogFolder);
            var d = new DirectoryInfo(folder);
            var models = new List<LoggingViewModel>();
            if (d.Exists)
            {
                // log_2017-04-27.log
                var extensions = new string[] { ".log",".json",".txt" };
                //DirectoryHelper.GetFileInfoes(folder,extensions,false)
                var files = d.EnumerateFiles().Where(f => f.Name.StartsWith($"{criteria.Logger}_"))
                    .Where(f => extensions.Contains(f.Extension.ToLowerInvariant()))
                    .Where(f => DateTime.Compare(f.CreationTime.Date,criteria.DateFrom.Date) >= 0)
                    .Where(f => DateTime.Compare(f.CreationTime.Date, criteria.DateTo.AddDays(1).Date) <= 0);

                foreach (var file in files)
                {
                    var messages = new List<string>();
                    using (var reader = file.OpenText())
                    {
                        while (reader.Peek() >= 0)
                        {
                            messages.Add(reader.ReadLine());
                        }                        
                    }
                    models.Add(new LoggingViewModel() { LoggingFileName = file.Name, FileSize = file.Length, Messages = messages.ToArray(), LoggingDateTime = file.LastWriteTime });
                }
                var totalRecords = models.Count;
                models = models.OrderByDescending(t => t.LoggingDateTime).ToList();
            }
            return JsonNet(models.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PurgeLogging(int days)
        {
            var folder = FileHelper.GetApplicationFullPath(ApplicationContext.Setting.ApplicationLogFolder);
            var d = new DirectoryInfo(folder);
            if (d.Exists)
            {
                var fileDeletes = new List<string>();
                try
                {
                    foreach (var file in d.EnumerateFiles())
                    {
                        if (DateTime.Now.Subtract(file.LastWriteTime).TotalDays >= days)
                        {
                            file.Delete();
                            fileDeletes.Add(file.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (fileDeletes.Count() == 0)
                        return InternalServerError(ex);
                }
            }
            return Ok("Purge logging message is successfully");
        }
    }
}