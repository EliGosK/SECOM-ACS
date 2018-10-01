using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Helper;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class SystemStatusController : AppControllerBase
    {
        private readonly IAccessControlService service;
        public SystemStatusController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("MAS040", PermissionNames.View)]
        [SiteMapPageTitle("MAS040")]
        public ActionResult Index()
        {
            var file = ApplicationContext.Setting.SuspendFile;
            var data = OfflineSystemFileManager.LoadFromFile(file) ?? OfflineOnlineSystemData.Default();
            return View(data);
        }

        public ActionResult Offline()
        {
            return View("_AppOffline");
        }
        
        [HttpPost]
        public ActionResult UpdateSchedule(OfflineOnlineSystemData data)
        {
            // Checking current date to set Start Effective Date. 
            //var actionTime = data.OnlineTime.TotalMinutes < data.OfflineTime.TotalMinutes ? data.OnlineTime : data.OfflineTime;
            data.StartEffectiveDate = DateTime.Now.Date;
            if (DateTime.Now.TimeOfDay.TotalMinutes > data.OfflineTime.TotalMinutes)
            {
                data.StartEffectiveDate = DateTime.Now.Date.AddDays(1);
            }
            var file = ApplicationContext.Setting.SuspendFile;
            OfflineSystemFileManager.WriteFile(file,data);
            data.IsUserOffline = false;
            data.Calculate();
            return JsonNet(new { message = MessageHelper.SaveCompleted(), data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSystemStatus(bool offline)
        {
            var file = ApplicationContext.Setting.SuspendFile;
            var data = OfflineSystemFileManager.LoadFromFile(file);
            data.IsUserOffline = offline;
            OfflineSystemFileManager.WriteFile(file, data);
            data.Calculate();
            return JsonNet(new { message = MessageHelper.SaveCompleted(), data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}