using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs args);
    public class TaskCompletedEventArgs : EventArgs
    {
        public TaskCompletedEventArgs(TimeSpan totalTime)
        {
            this.TotalTime = totalTime;
        }

        public TaskCompletedEventArgs(TimeSpan totalTime,Exception error)
        {
            this.TotalTime = totalTime;
            this.Error = error;
        }

        public TimeSpan TotalTime { get; private set; }
        public Exception Error { get; private set; }
        public bool IsSuccess { get { return this.Error == null; } }
    }

    public delegate void TaskProgressEventHandler(object sender, TaskProgressEventArgs args);
    public class TaskProgressEventArgs : EventArgs
    {
        public TaskProgressEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }
    }
}
