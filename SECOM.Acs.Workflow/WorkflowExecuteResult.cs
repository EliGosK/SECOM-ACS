using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public class WorkflowExecuteResult
    {
        public WorkflowExecuteResult()
        {

        }

        public WorkflowExecuteResult(Exception ex)
        {
            this.Error = ex;
        }

        public Exception Error { get; private set; }
        public bool IsSucceed
        {
            get { return this.Error == null; }
        }

        public static WorkflowExecuteResult Succeed()
        {
            return new WorkflowExecuteResult();
        }

        public static WorkflowExecuteResult Fail(Exception ex)
        {
            return new WorkflowExecuteResult(ex);
        }
    }
}
