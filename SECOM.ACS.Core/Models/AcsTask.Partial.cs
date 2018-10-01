using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AcsTask
    {
        public Exception Error { get; set; }
        public bool IsSuccess { get { return this.Error == null; } }

        public AcsTaskSchedule[] Schedules { get; set; }

        public bool CanRun { get; set; }
        public bool CanEdit { get; set; }
    }

    public class AcsTaskSchedule
    {
        public string CronExpression { get; set; }
        public string Description { get; set; }
    }
}
