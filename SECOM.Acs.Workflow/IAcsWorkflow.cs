using CSI.ComponentModel;
using SECOM.ACS.Models;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public interface IAcsWorkflow
    {
        Guid InstanceId { get; }
        void StartForCreateRequest(IAcsRequest request);
        void StartForApprovalRequest(WorkflowDataState dataState, ExportInterfaceFileOptions exportInterfaceFileOptions);
        void StartForCancelRequest(IAcsRequest request, ExportInterfaceFileOptions exportInterfaceFileOptions);

        event MessageEventHandler Progress;
        event ErrorEventHandler Error;
    }

    

    
}
