using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public class WorkflowDataState
    {
        public WorkflowDataState(Guid dataId, IAcsRequest request)
        {
            this.DataId = dataId;
            this.Request = request;
        }

        public Guid DataId { get; private set; }
        public IAcsRequest Request { get; private set; }
    }
}
