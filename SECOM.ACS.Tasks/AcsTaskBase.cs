using CSI.ComponentModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace SECOM.ACS.Tasks
{
    public abstract class AcsTaskBase<TOption> : IAcsTask<TOption>, IDisposable
    {
        public AcsTaskBase(string taskId,string taskName)
        {
            this.TaskID = taskId;
            this.TaskName = taskName;
        }

        private object startEventKey = new object();
        private object errorEventKey = new object();
        private object completedEventKey = new object();
        private object progressEventKey = new object();
        private EventHandlerList handlers = new EventHandlerList();

        public string TaskID { get; private set; }
        public string TaskName { get; private set; }

        public event EventHandler Started
        {
            add { handlers.AddHandler(startEventKey, value); }
            remove { handlers.RemoveHandler(startEventKey, value); }
        }

        public event ErrorEventHandler Error
        {
            add { handlers.AddHandler(errorEventKey, value); }
            remove { handlers.RemoveHandler(errorEventKey, value); }
        }

        public event TaskCompletedEventHandler Completed
        {
            add { handlers.AddHandler(completedEventKey, value); }
            remove { handlers.RemoveHandler(completedEventKey, value); }
        }

        public event TaskProgressEventHandler Progress
        {
            add { handlers.AddHandler(progressEventKey, value); }
            remove { handlers.RemoveHandler(progressEventKey, value); }
        }

        public ObjectResult Execute(TOption options)
        {
            var startTime = Stopwatch.StartNew();
            OnStarted(EventArgs.Empty);
            try
            {
                var userState = ExecuteTask(options);
                startTime.Stop();
                OnCompleted(new TaskCompletedEventArgs(startTime.Elapsed));
                return ObjectResult.Succeed(userState);
            }
            catch (Exception ex)
            {
                startTime.Stop();
                OnCompleted(new TaskCompletedEventArgs(startTime.Elapsed,ex));
                return ObjectResult.Fail(ex);
            }
        }

        protected abstract object ExecuteTask(TOption options);

        protected void OnStarted(EventArgs e)
        {
            var handler = handlers[startEventKey] as EventHandler;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected void OnProgress(TaskProgressEventArgs e)
        {
            var handler = handlers[progressEventKey] as TaskProgressEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnError(ErrorEventArgs e)
        {
            var handler = handlers[errorEventKey] as ErrorEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnCompleted(TaskCompletedEventArgs e)
        {
            var handler = handlers[completedEventKey] as TaskCompletedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void Dispose()
        {
           
        }
    }


    
}
