using CSI.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    [ModelBinder(typeof(OfflineOnlineSystemDataModelBinder))]
    public class OfflineOnlineSystemData
    {
        public TimeSpan OfflineTime { get; set; }
        public TimeSpan OnlineTime { get; set; }
        public DateTime StartEffectiveDate { get; set; } = DateTime.Now;
        public bool IsUserOffline { get; set; }

        public DateTime NextOfflineTime { get; private set; }
        public DateTime NextOnlineTime { get; private set; }
        public bool IsOfflineSystem { get; private set; }

        public bool EnabledOffline { get; set; } 


        public static OfflineOnlineSystemData Default()
        {
            return new OfflineOnlineSystemData()
            {
                StartEffectiveDate = DateTime.Now.Date,
                IsUserOffline = true,
                OfflineTime = new TimeSpan(22,30,0),
                OnlineTime = new TimeSpan(6,0,0)
            };
        }

        public void Calculate()
        {
            var startDate = DateTime.Now < this.StartEffectiveDate ? this.StartEffectiveDate.Date : DateTime.Now.Date;
            this.NextOfflineTime = startDate.AddMinutes(this.OfflineTime.TotalMinutes);
            this.NextOnlineTime = startDate.AddMinutes(this.OnlineTime.TotalMinutes);

            if (this.OfflineTime.TotalMinutes > this.OnlineTime.TotalMinutes)
            {
                this.NextOnlineTime = startDate.AddDays(1).AddMinutes(this.OnlineTime.TotalMinutes);
            }
            this.IsOfflineSystem = this.IsUserOffline || (DateTime.Now.Ticks >= this.NextOfflineTime.Ticks && DateTime.Now.Ticks < this.NextOnlineTime.Ticks);

            if (!this.EnabledOffline)
            {
                this.IsOfflineSystem = false;
            }
        }
    }

    public class OfflineOnlineSystemDataModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                           ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as OfflineOnlineSystemData;
            if (model != null)
            {
                var value = (string)controllerContext.HttpContext.Request["OfflineTime"];
                if (!String.IsNullOrEmpty(value)) {
                    DateTime d;
                    DateTime.TryParse(value,  CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out d);
                    model.OfflineTime = new TimeSpan(d.Hour, d.Minute, 0);
                }

                value = (string)controllerContext.HttpContext.Request["OnlineTime"];
                if (!String.IsNullOrEmpty(value))
                {
                    DateTime d;
                    DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out d);
                    model.OnlineTime = new TimeSpan(d.Hour, d.Minute, 0);
                }
            }
            return model;

        }
    }
}