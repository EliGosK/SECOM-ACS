using CSI.ComponentModel;
using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Workflow
{
    public class WorkflowManager : IDisposable
    {
        protected Dictionary<string, Type> workflowMappings = new Dictionary<string, Type>();
        private Dictionary<Type, object> internalServices = new Dictionary<Type, object>();

        private EventHandlerList handlers = new EventHandlerList();
        private object workflowStartedEventKey = new object();
        private object workflowErrorEventKey = new object();
        private object workflowCompletedEventKey = new object();
        private object workflowProgressEventKey = new object();
        private object workflowCreatedEventKey = new object();

        #region Events
        public event WorkflowCreatedEventHandler WorkflowCreated
        {
            add { this.handlers.AddHandler(workflowCreatedEventKey, value); }
            remove { this.handlers.RemoveHandler(workflowCreatedEventKey, value); }
        }

        public event WorkflowStartedEventHandler WorkflowStarted
        {
            add { this.handlers.AddHandler(workflowStartedEventKey, value); }
            remove { this.handlers.RemoveHandler(workflowStartedEventKey, value); }
        }

        
        public event WorkflowProgressEventHandler WorkflowProgress
        {
            add { this.handlers.AddHandler(workflowProgressEventKey, value); }
            remove { this.handlers.RemoveHandler(workflowProgressEventKey, value); }
        }

        public event WorkflowErrorEventHandler WorkflowError
        {
            add { this.handlers.AddHandler(workflowErrorEventKey, value); }
            remove { this.handlers.RemoveHandler(workflowErrorEventKey, value); }
        }


        public event WorkflowCompletedEventHandler WorkflowCompleted
        {
            add { this.handlers.AddHandler(workflowCompletedEventKey, value); }
            remove { this.handlers.RemoveHandler(workflowCompletedEventKey, value); }
        }
        #endregion

        public void AddService(object service)
        {
            internalServices.Add(service.GetType(), service);
        }

        public object GetService(Type type)
        {
            return internalServices.Where(t => type.IsAssignableFrom(t.Key))
                .Select(t=>t.Value)
                .FirstOrDefault();
        }

        public object GetService<T>()
        {
            return internalServices.Where(t => typeof(T).IsAssignableFrom(t.Key))
                .Select(t => t.Value)
                .FirstOrDefault();
        }

        public object[] GetAllServices<T>()
        {
            return internalServices.Where(t => typeof(T).IsAssignableFrom(t.Key))
                .Select(t => t.Value)
                .ToArray();
        }

        public object[] GetAllServices(Type type)
        {
            return internalServices.Where(t => type.IsAssignableFrom(t.Key))
                .Select(t => t.Value)
                .ToArray();
        }

        public object[] GetAllServices()
        {
            return internalServices
                .Select(t => t.Value)
                .ToArray();
        }
       
        protected IAcsWorkflow CreateWorkflowFromAcsRequest(IAcsRequest request)
        {
            var documentType = request.DocumentType;
            if (!workflowMappings.ContainsKey(documentType))
            {
                throw new InvalidOperationException($"Acs workflow mapping is not found. Document type code {documentType}.");
            }
            return this.CreateWorkflow(workflowMappings[documentType], documentType);
        }

        private IAcsWorkflow CreateWorkflow(Type workflowType,string description = null)
        {
            var ctors = workflowType.GetConstructors();
            if (ctors.Count() > 0)
            {
                foreach (var ctor in ctors)
                {
                    var parameters = ctor.GetParameters();
                    var workflow = Activator.CreateInstance(workflowType, parameters.Select(t => GetService(t.ParameterType)).ToArray()) as IAcsWorkflow;
                    OnWorkflowCreated(new WorkflowCreatedEventArgs(workflow, description));
                    AttachEvents(workflow);
                    return workflow;
                }
                return null;
            }
            else
            {
                // Default Consturcturs
                var workflow = Activator.CreateInstance(workflowType) as IAcsWorkflow;
                OnWorkflowCreated(new WorkflowCreatedEventArgs(workflow, description));
                AttachEvents(workflow);
                return workflow;
            }
        }

        private void AttachEvents(IAcsWorkflow workflow)
        {
            workflow.Error += delegate (object sender, ErrorEventArgs e)
            {
                OnWorkflowError(new WorkflowErrorEventArgs(workflow, e.GetException()));
            };

            workflow.Progress += delegate (object sender, MessageEventArgs e)
            {
                OnWorkflowProgress(new WorkflowProgressEventArgs(workflow, e.Message));
            };
        }


        public void RegisterWorkflow(string name, Type workflow)
        {
            if (workflowMappings.ContainsKey(name))
                workflowMappings.Remove(name);
            workflowMappings.Add(name, workflow);
        }

        public WorkflowExecuteResult RunForCreateRequest(IAcsRequest request)
        {
            var startTime = Stopwatch.StartNew();
            var workflowInstance = CreateWorkflowFromAcsRequest(request);
            OnWorkflowStarted(new WorkflowStartedEventArgs(workflowInstance));
            try
            {
                workflowInstance.StartForCreateRequest(request);
                startTime.Stop();
                OnWorkflowCompleted(new WorkflowCompletedEventArgs(workflowInstance, startTime.Elapsed));
                return WorkflowExecuteResult.Succeed();
            }
            catch (Exception ex)
            {
                OnWorkflowError(new WorkflowErrorEventArgs(workflowInstance,ex));
                return WorkflowExecuteResult.Fail(ex);
            }
        }

        public WorkflowExecuteResult RunForApprovalRequest(WorkflowDataState state,ExportInterfaceFileOptions exportInterfaceFileOptions)
        {
            var startTime = Stopwatch.StartNew();
            var workflowInstance = CreateWorkflowFromAcsRequest(state.Request);
            OnWorkflowStarted(new WorkflowStartedEventArgs(workflowInstance));
            try
            {
                workflowInstance.StartForApprovalRequest(state, exportInterfaceFileOptions);
                startTime.Stop();
                OnWorkflowCompleted(new WorkflowCompletedEventArgs(workflowInstance, startTime.Elapsed));
                return WorkflowExecuteResult.Succeed();
            }
            catch (Exception ex)
            {
                OnWorkflowError(new WorkflowErrorEventArgs(workflowInstance,ex));
                return WorkflowExecuteResult.Fail(ex);
            }
            
        }

        public WorkflowExecuteResult RunForCancelRequest(IAcsRequest request, ExportInterfaceFileOptions exportInterfaceFileOptions)
        {
            var startTime = Stopwatch.StartNew();
            var workflowInstance = CreateWorkflowFromAcsRequest(request);
            OnWorkflowStarted(new WorkflowStartedEventArgs(workflowInstance));
            try
            {
              
                workflowInstance.StartForCancelRequest(request, exportInterfaceFileOptions);
                startTime.Stop();
                OnWorkflowCompleted(new WorkflowCompletedEventArgs(workflowInstance, startTime.Elapsed));
                return WorkflowExecuteResult.Succeed();
            }
            catch (Exception ex)
            {
                OnWorkflowError(new WorkflowErrorEventArgs(workflowInstance,ex));
                return WorkflowExecuteResult.Fail(ex);
            }
           
        }

        public void Dispose()
        {
         
        }

        #region Raise Events        
        protected void OnWorkflowCreated(WorkflowCreatedEventArgs e)
        {
            var handler = handlers[workflowCreatedEventKey] as WorkflowCreatedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowStarted(WorkflowStartedEventArgs e)
        {
            var handler = handlers[workflowStartedEventKey] as WorkflowStartedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowProgress(WorkflowProgressEventArgs e)
        {
            var handler = handlers[workflowProgressEventKey] as WorkflowProgressEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowError(WorkflowErrorEventArgs e)
        {
            var handler = handlers[workflowErrorEventKey] as WorkflowErrorEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowCompleted(WorkflowCompletedEventArgs e)
        {
            var handler = handlers[workflowCompletedEventKey] as WorkflowCompletedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
