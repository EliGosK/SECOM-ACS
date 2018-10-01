using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public class WorkflowEventArgs : EventArgs
    {
        public WorkflowEventArgs(IAcsWorkflow workflow)
        {
            this.WorkflowInstance = workflow;
        }

        public IAcsWorkflow WorkflowInstance { get; private set; }
    }

    public delegate void WorkflowCreatedEventHandler(object sender, WorkflowCreatedEventArgs e);
    public class WorkflowCreatedEventArgs : WorkflowEventArgs
    {
        public WorkflowCreatedEventArgs(IAcsWorkflow workflow,string description) : base(workflow)
        {
            this.Description = description;
        }

        public string Description { get; private set; }
    }

    public delegate void WorkflowStartedEventHandler(object sender, WorkflowStartedEventArgs e);
    public class WorkflowStartedEventArgs : WorkflowEventArgs
    {
        public WorkflowStartedEventArgs(IAcsWorkflow workflow) : base(workflow)
        {

        }
    }

    public delegate void WorkflowProgressEventHandler(object sender, WorkflowProgressEventArgs e);
    public class WorkflowProgressEventArgs : WorkflowEventArgs
    {
        public WorkflowProgressEventArgs(IAcsWorkflow workflow, string message) : base(workflow)
        {
            this.Message = message;
        }
        public string Message { get; private set; }
    }

    public delegate void WorkflowErrorEventHandler(object sender, WorkflowErrorEventArgs e);
    public class WorkflowErrorEventArgs : WorkflowEventArgs
    {
        public WorkflowErrorEventArgs(IAcsWorkflow workflow, Exception error) : base(workflow)
        {
            this.Error = error;
        }

        public Exception Error { get; private set; }
    }

    public delegate void WorkflowCompletedEventHandler(object sender, WorkflowCompletedEventArgs e);
    public class WorkflowCompletedEventArgs : WorkflowEventArgs
    {
        public WorkflowCompletedEventArgs(IAcsWorkflow workflow,TimeSpan totalTimes) : base(workflow)
        {
            this.TotalUsageTimes = totalTimes;
        }

        public TimeSpan TotalUsageTimes { get; private set; }
    }
}
