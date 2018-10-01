using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using RS = SECOM.ACS.MvcWebApp.Properties;
using System.Web.Mvc;
using SECOM.ACS.MvcWebApp.Extensions;
using CSI.Web.Mvc;
using CSI.Localization;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [RoutePrefix("api/items")]
    [RouteCulture]
    public class ItemApiController : AppControllerBase
    {
        private readonly IAccessControlService service;

        public ItemApiController(IAccessControlService service)
        {
            this.service = service;
        }
        
        [Route("{id:int}")]
        public ActionResult GetItem(int id)
        {
            var dataItem = service.GetItem(id);
            return JsonNet(dataItem, JsonRequestBehavior.AllowGet);
        }

        [Route("list")]
        public ActionResult GetListItem()
        {
            var dataItems = service.GetAllItems()
                .Select(t => new
                {
                    ItemID = t.ItemID,
                    ItemTypeID = t.ItemTypeID,
                    ItemName = t.ItemName,
                    ItemDisplay = ModelLocalizeManager.GetValue(t, "ItemDisplay")
                }).ToList()
                 .OrderBy(t => t.ItemDisplay, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true));
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("list/Out")]
        public ActionResult GetListItemOut()
        {
            var dataItems = service.GetAllItems()
                .Select(t => new
                {
                    ItemID = t.ItemID,
                    ItemTypeID = t.ItemTypeID,
                    ItemName = t.ItemName,
                    ItemIsItemOut = t.IsItemOut,
                    IsConfdt = t.IsConfdt,
                    IsActive = t.IsActive,
                    ItemDisplay = ModelLocalizeManager.GetValue(t, "ItemDisplay")
                })
                .Where(t => t.ItemIsItemOut == true && t.IsActive == true).ToList()
                .OrderBy(t => t.ItemDisplay,StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true));
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }

        [Route("list/In")]
        public ActionResult GetListItemIn()
        {
            var dataItems = service.GetAllItems()
                .Select(t => new
                {
                    ItemID = t.ItemID,
                    ItemTypeID = t.ItemTypeID,
                    ItemName = t.ItemName,
                    ItemIsItemIn = t.IsItemIn,
                    IsConfdt = t.IsConfdt,
                    ItemDisplay = ModelLocalizeManager.GetValue(t, "ItemDisplay"),
                    IsActive = t.IsActive
                })
                .Where(t => t.ItemIsItemIn == true && t.IsActive == true).ToList()
                 .OrderBy(t => t.ItemDisplay, StringComparer.Create(System.Threading.Thread.CurrentThread.CurrentUICulture, true));
            return JsonNet(dataItems, JsonRequestBehavior.AllowGet);
        }
    }
}