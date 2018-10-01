using CSI.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public interface IAcsTask<TOption>
    {
        string TaskID { get; }
        string TaskName { get; }

        event EventHandler Started;
        event ErrorEventHandler Error;
        event TaskCompletedEventHandler Completed;
        event TaskProgressEventHandler Progress;

        ObjectResult Execute(TOption options);
    }
}
