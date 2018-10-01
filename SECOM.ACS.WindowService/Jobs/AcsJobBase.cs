using Common.Logging;
using CSI.Exceptions;
using Quartz;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.WindowsService
{
    public abstract class AcsJobBase<TOptions> : IJob
    {
        protected readonly IAcsTask<TOptions> task;

        public AcsJobBase(IAcsTask<TOptions> task)
        {
            this.task = task;
        }

        public abstract void Execute(IJobExecutionContext context);
    }
}
