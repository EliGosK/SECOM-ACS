using Common.Logging;
using CronExpressionDescriptor;
using CSI.ComponentModel;
using CSI.Exceptions;
using CSI.IO;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using SECOM.ACS.Web.Mvc;
using SECOM.ACS.WindowsService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class ScheduleTaskController : AppControllerBase
    {
        private readonly IAccessControlService service;
        private readonly IAcsTask<UpdateEmployeeInfoTaskOptions> updateEmployeeInfoTask;
        private readonly IAcsTask<UpdateDocumentStatusTaskOptions> updateDocumentExpirationTask;
        private readonly IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> exportToAccessControlTask;
        private readonly IAcsTask<TransferInterfaceFileToAccessControlTaskOptions> importToAccessControlTask;

        public ScheduleTaskController(IAccessControlService service,
            IAcsTask<UpdateEmployeeInfoTaskOptions> updateEmployeeInfoTask,
            IAcsTask<UpdateDocumentStatusTaskOptions> updateDocumentExpirationTask,
            IAcsTask<ExportInterfaceFileToAccessControlTaskOptions> exportToAccessControlTask,
            IAcsTask<TransferInterfaceFileToAccessControlTaskOptions> importToAccessControlTask)
        {
            this.service = service;
            this.updateEmployeeInfoTask = updateEmployeeInfoTask;
            this.updateDocumentExpirationTask = updateDocumentExpirationTask;
            this.exportToAccessControlTask = exportToAccessControlTask;
            this.importToAccessControlTask = importToAccessControlTask;
            
            AttachTaskEvent(this.updateEmployeeInfoTask, LogManager.GetLogger(this.updateEmployeeInfoTask.TaskID));
            AttachTaskEvent(this.updateDocumentExpirationTask, LogManager.GetLogger(this.updateDocumentExpirationTask.TaskID));
            AttachTaskEvent(this.exportToAccessControlTask, LogManager.GetLogger(this.exportToAccessControlTask.TaskID));
        }

        [ApplicationAuthorize("SYS050", PermissionNames.View)]
        [SiteMapPageTitle("SYS050")]
        public ActionResult Index()
        {
            return View();
        }

        private AcsTaskAttribute GetAttribute(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(AcsTaskAttribute), true) as AcsTaskAttribute[];
            if (attrs == null || attrs.Length == 0)
                return new AcsTaskAttribute();
            return attrs[0];
        }

        [HttpPost]
        public ActionResult ListTasks([DataSourceRequest]DataSourceRequest request)
        {
            //var dataItems = service.GetAllAcsTask();
            //var result = dataItems.ToDataSourceResult(request, (AcsTask task) => task.ToViewModel());
            //return JsonNet(result, JsonRequestBehavior.AllowGet);

            try
            {
                var dataItems = service.GetAllAcsTask();
                var serviceConfigFile = FileHelper.GetApplicationFullPath(ApplicationContext.Setting.HostedProcess.ConfigFile);
                if (System.IO.File.Exists(serviceConfigFile))
                {
                    var options = AcsScheduleServiceOptions.LoadFromJsonFile(serviceConfigFile);

                    foreach (var dataItem in dataItems)
                    {
                        var attr = new AcsTaskAttribute();
                        switch (dataItem.TaskID.ToLowerInvariant())
                        {
                            case "acp010":
                                attr = GetAttribute(typeof(UpdateEmployeeInfoTask));
                                dataItem.Schedules = options.ImportEmployeeOptions.CronExpressions.Select(t => new AcsTaskSchedule
                                {
                                    CronExpression = t,
                                    Description = ExpressionDescriptor.GetDescription(t, new Options() { Use24HourTimeFormat = true })
                                }).OrderBy(t => t.Description).ToArray();
                                break;
                            case "acp020":
                                attr = GetAttribute(typeof(UpdateDocumentStatusTask));
                                dataItem.Schedules = options.UpdateDocumentStatusOptions.CronExpressions.Select(t => new AcsTaskSchedule
                                {
                                    CronExpression = t,
                                    Description = ExpressionDescriptor.GetDescription(t, new Options() { Use24HourTimeFormat = true })
                                }).OrderBy(t => t.Description).ToArray();
                                break;
                            case "acp030":
                                attr = GetAttribute(typeof(ExportInterfaceFileToAccessControlTask));
                                dataItem.Schedules = options.ExportInterfaceFileToAccessControlOptions.CronExpressions.Select(t => new AcsTaskSchedule
                                {
                                    CronExpression = t,
                                    Description = ExpressionDescriptor.GetDescription(t, new Options() { Use24HourTimeFormat = true })
                                }).OrderBy(t => t.Description).ToArray();
                                break;
                            case "acp040":
                                attr = GetAttribute(typeof(TransferInterfaceFileToAccessControlTask));
                                dataItem.Schedules = options.TransferInterfaceFileToAccessControlOptions.CronExpressions.Select(t => new AcsTaskSchedule
                                {
                                    CronExpression = t,
                                    Description = ExpressionDescriptor.GetDescription(t, new Options() { Use24HourTimeFormat = true })
                                }).OrderBy(t => t.Description).ToArray();
                                break;
                            default:
                                break;
                        }

                        dataItem.CanEdit = attr.CanEdit;
                        dataItem.CanRun = attr.CanRun;
                    }
                }
                var result = dataItems.ToDataSourceResult(request, (AcsTask task) => task.ToViewModel());
                return JsonNet(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public ActionResult UpdateScheduleTime(AcsTaskViewModel model)
        {
            var serviceConfigFile = FileHelper.GetApplicationFullPath(ApplicationContext.Setting.HostedProcess.ConfigFile);
            if (!System.IO.File.Exists(serviceConfigFile))
            {
                return InternalServerError(new Exception($"Service configuration file {serviceConfigFile} not found."));
            }

            try
            {
                var options = AcsScheduleServiceOptions.LoadFromJsonFile(serviceConfigFile);
                switch (model.TaskID.ToLowerInvariant())
                {
                    case "acp010":
                        options.ImportEmployeeOptions.CronExpressions = model.Schedules.Select(t => t.CronExpression).ToArray();
                        break;
                    case "acp020":
                        options.UpdateDocumentStatusOptions.CronExpressions = model.Schedules.Select(t => t.CronExpression).ToArray();
                        break;
                    case "acp030":
                        options.ExportInterfaceFileToAccessControlOptions.CronExpressions = model.Schedules.Select(t => t.CronExpression).ToArray();
                        break;
                    default:
                        break;
                }
                AcsScheduleServiceOptions.SaveConfiguration(serviceConfigFile, options);
                return Ok(MessageHelper.SaveCompleted());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
           
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ApplicationSuspend]
        public ActionResult RunAllTask()
        {
            var tasks = new string[] { "acp010", "acp020", "acp030" };
            foreach (var taskId in tasks)
            {               
                ObjectResult result = ObjectResult.Succeed();
                switch (taskId)
                {
                    case "acp010":
                        result = this.updateEmployeeInfoTask.Execute(ApplicationContext.Setting.Task.ToUpdateEmployeeInfoTaskOptions());
                        break;
                    case "acp020":
                        result = this.updateDocumentExpirationTask.Execute(ApplicationContext.Setting.Task.ToUpdateDocumentExpirationOptions());
                        break;
                    case "acp030":
                        var taskOptions = ApplicationContext.Setting.Task.ToExportToAccessControlTaskOptions();
                        taskOptions.TaskOptions.EffectiveDate = DateTime.Now.Date;
                        result = this.exportToAccessControlTask.Execute(taskOptions);
                        break;
                }

                if (result.IsSucceed)
                {
                    var batchToUpdated = new AcsTask() { TaskID = taskId, LastResultMessage = "The batch task was executed successfully.", UpdateBy = User.Identity.Name };
                    service.UpdateAcsTask(batchToUpdated);
                }
                else
                {
                    service.UpdateAcsTask(new AcsTask() { TaskID = taskId, LastResultMessage = result.GetErrorMessage(), UpdateBy = User.Identity.Name, Error = result.Error });
                }
            }
            return Ok("The batch task was executed successfully.");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ViewLogMessage(string taskId)
        {
            var folder = FileHelper.GetApplicationFullPath(ApplicationContext.Setting.HostedProcess.LoggingFolder);
            var file = DirectoryHelper.GetFileInfoInDirectory(folder, new string[] { "txt", "log" }, false)
                .Where(t => t.Name.StartsWith(taskId, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(t => t.LastAccessTime)
                .FirstOrDefault();

            if (file == null)
            {
                return InternalServerError($"Log message not found for Task ID: {taskId}");
            }

            var messages = new List<string>();
            using (var reader = System.IO.File.OpenText(file.FullName))
            {
                while (reader.Read() >= 0)
                {
                    messages.Add(reader.ReadLine());
                }
            }

            var fileName = System.IO.Path.GetFileName(file.FullName);
            return JsonNet(new { TaskID = taskId, LoggingMessages = messages, LogFile = fileName }, JsonRequestBehavior.AllowGet);
        }

        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult RunTask(string taskId)
        {
            if (String.IsNullOrEmpty(taskId)) { return InternalServerError("Invalid request."); }

            taskId = taskId.ToLowerInvariant();
            var logger = LogManager.GetLogger(taskId);
            ObjectResult result = ObjectResult.Succeed();
            switch (taskId)
            {
                case "acp010":
                    result = this.updateEmployeeInfoTask.Execute(ApplicationContext.Setting.Task.ToUpdateEmployeeInfoTaskOptions());
                    break;
                case "acp020":
                    result = this.updateDocumentExpirationTask.Execute(ApplicationContext.Setting.Task.ToUpdateDocumentExpirationOptions());
                    break;
                case "acp030":                   
                    result = this.exportToAccessControlTask.Execute(ApplicationContext.Setting.Task.ToExportToAccessControlTaskOptions());
                    break;
                default:
                    return InternalServerError("$Invalid task id {taskId}.");
            }

            if (result.IsSucceed)
            {
                return Ok("The batch task was executed successfully.");
            }
            else
            {
                return InternalServerError(result.GetErrorMessage());
            }

        }

        protected void AttachTaskEvent<TOptions>(IAcsTask<TOptions> task, ILog logger)
        {
            task.Started += delegate (object sender, EventArgs e)
            {
                logger.Info($"Task {task.TaskID}:{task.TaskName} is started.");
            };

            task.Progress += delegate (object sender, TaskProgressEventArgs e)
            {
                logger.Info(e.Message);
            };

            task.Error += delegate (object sender, ErrorEventArgs e)
            {
                logger.Error($"Error occured. Error message: {ExceptionUtility.GetLastExceptionMessage(e.GetException())}");
                logger.Error($"StackTrace: {e.GetException().StackTrace}");
            };

            task.Completed += delegate (object sender, TaskCompletedEventArgs e)
            {
                logger.Info($"Task {task.TaskID}:{task.TaskName} is execute completed. Execute result {(e.Error == null ? "SUCCESS" : "FAILED")} .Total execute {e.TotalTime.TotalMinutes} minutes");
                if (e.Error != null)
                {
                    logger.Error($"Task {task.TaskID}:{task.TaskName} is error occured.", e.Error);
                }
                var user = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                if (e.IsSuccess)
                {
                    var batchToUpdated = new AcsTask() { TaskID = task.TaskID, LastResultMessage = "The batch task was executed successfully.", UpdateBy = user };
                    service.UpdateAcsTask(batchToUpdated);
                }
                else
                {
                    var message = ExceptionUtility.GetLastExceptionMessage(e.Error);
                    service.UpdateAcsTask(new AcsTask() { TaskID = task.TaskID, LastResultMessage = message, UpdateBy = user, Error = e.Error });
                }
            };
        }

        public ActionResult GetServiceStatus()
        {
            string serviceName = ApplicationContext.Setting.HostedProcess.ServiceName;
            var service = new ServiceController(serviceName);
            try
            {
                return JsonNet(new { serviceName = service.ServiceName, status = service.Status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public ActionResult StartService(bool start)
        {
            string serviceName = ApplicationContext.Setting.HostedProcess.ServiceName;
            var service = new ServiceController(serviceName);
            int timeoutMilliseconds = 10 * 1000;
            try
            {
                if (start)
                {
                    service.Start();
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    return JsonNet(new { message = "Window Service is started.", status = service.Status },JsonRequestBehavior.AllowGet);
                }

                if (!start)
                {
                    service.Stop();
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    return JsonNet(new { message = "Window Service is stop.", status = service.Status },JsonRequestBehavior.AllowGet);
                }
                return InternalServerError($"Window Service is busy. Current Status {service.Status}");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public ActionResult RestartService()
        {
            string serviceName = ApplicationContext.Setting.HostedProcess.ServiceName;
            int timeoutMilliseconds = 5 * 1000;
            var service = new ServiceController(serviceName);
            try
            {
                
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return Ok($"Restart windows service name {serviceName} is successfully.");
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}