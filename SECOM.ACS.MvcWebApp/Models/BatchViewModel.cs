using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Models
{
    public class AcsTaskViewModel
    {
        public string TaskID { get; set; }
        public string TaskName { get; set; }
        public DateTime? LastRunDateTime { get; set; }
        public string LastResultMessage { get; set; }
        public bool IsLastResultSuccess { get; set; }
        public int Success { get; set; }
        public int Fail { get; set; }
        public bool CanRun { get; set; }
        public bool CanEdit { get; set; }
        public AcsTaskSchedule[] Schedules { get; set; }

        public int Total
        {
            get {
                return this.Success + this.Fail;
            }
        }

    }
}