using Atlas;
using Autofac;
using Common.Logging;
using CSI.Exceptions;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.IO;
using System.Reflection;
using Module = Autofac.Module;

namespace SECOM.ACS.WindowsService
{
    /// <summary>
    /// This represents an entity for Autofac module.
    /// </summary>
    internal class AccessControlModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            this.LoadQuartz(builder);
            this.LoadServices(builder);
            this.LoadLogicLayers(builder);
        }

        /// <summary>
        /// Loads the quartz scheduler instance.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadQuartz(ContainerBuilder builder)
        {
            builder.Register(c => new StdSchedulerFactory().GetScheduler())
                   .As<IScheduler>()
                   .InstancePerLifetimeScope(); // #1
            builder.Register(c => new AccessControlJobFactory(ContainerProvider.Instance.ApplicationContainer))
                   .As<IJobFactory>();          // #2
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(p => typeof(IJob).IsAssignableFrom(p))
                   .PropertiesAutowired();      // #3
            builder.Register(c => new AccessControlJobListener(ContainerProvider.Instance))
                   .As<IJobListener>();         // #4
        }

        /// <summary>
        /// Loads the service instance.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<AccessControlHostedProcess>()
                   .As<IAmAHostedProcess>()
                   .PropertiesAutowired();      // #5
        }

        /// <summary>
        /// Loads the logic layers.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        private void LoadLogicLayers(ContainerBuilder builder)
        {
            var service = new AccessControlService ();
            var interfaceService = new DataInterfaceService();
            var documentExpirationService = new DocumentService();
            var mailProvider = new RazorMailProvider(new RazorMailOptions() { BaseTemplateFolder = "EmailTemplates" });

            builder.RegisterInstance(service)
               .As<IMasterService>();

            var updateEmployeeInfoTask = new UpdateEmployeeInfoTask(service, interfaceService);
            AttachTaskEvent(updateEmployeeInfoTask, LogManager.GetLogger(updateEmployeeInfoTask.TaskID.ToLowerInvariant()), service);
            builder.RegisterInstance(updateEmployeeInfoTask)
                .As<IAcsTask<UpdateEmployeeInfoTaskOptions>>();

            var updateDocumentExpirationTask = new UpdateDocumentStatusTask(service, documentExpirationService, mailProvider);
            AttachTaskEvent(updateDocumentExpirationTask, LogManager.GetLogger(updateDocumentExpirationTask.TaskID.ToLowerInvariant()), service);
            builder.RegisterInstance(updateDocumentExpirationTask)
                .As<IAcsTask<UpdateDocumentStatusTaskOptions>>();

            var exportToAccessControlTask = new ExportInterfaceFileToAccessControlTask(interfaceService,service);
            AttachTaskEvent(exportToAccessControlTask, LogManager.GetLogger(exportToAccessControlTask.TaskID.ToLowerInvariant()), service);
            builder.RegisterInstance(exportToAccessControlTask)
                .As<IAcsTask<ExportInterfaceFileToAccessControlTaskOptions>>();

            var importToAccessControlTask = new TransferInterfaceFileToAccessControlTask();
            AttachTaskEvent(importToAccessControlTask, LogManager.GetLogger(importToAccessControlTask.TaskID.ToLowerInvariant()), service);
            builder.RegisterInstance(importToAccessControlTask)
                .As<IAcsTask<TransferInterfaceFileToAccessControlTaskOptions>>();
        }

        private void AttachTaskEvent<TOptions>(IAcsTask<TOptions> task, ILog logger, IMasterService service)
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
                logger.Info($"Task {task.TaskID}:{task.TaskName} is execute completed. Execute result {(e.Error==null?"SUCCESS":"FAILED")} .Total execute {e.TotalTime.TotalMinutes} minutes");
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
    }
}