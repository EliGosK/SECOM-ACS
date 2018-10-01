using Common.Logging;
using CSI.Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Practices.Unity;
using Owin;
using SECOM.ACS.Identity;
using SECOM.ACS.Infrastructure;
using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.App_Start;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using SECOM.ACS.Workflow;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

[assembly: OwinStartup(typeof(SECOM.ACS.MvcWebApp.Startup))]

namespace SECOM.ACS.MvcWebApp
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {

            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.CreatePerOwinContext<ACSContext>(() => {
                var context = new ACSContext();
                // Caching System Miscs (Active Only)
                ApplicationContext.DataContext.LoadSystemMisc(context.SystemMiscs.ToList());         
                return context;
            });

            app.CreatePerOwinContext<IdentityContext>(() => {
                var context = new IdentityContext();
                context.Seed();
                return context;
            });

            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationOwinContextManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationOwinContextManager.Create);

            // Mail
            app.CreatePerOwinContext<MailManager>(()=> {
                var folder = ApplicationContext.Setting.Mail.MailTemplateFolder;
                var manager = new MailManager(new RazorMailProvider(new RazorMailOptions() { BaseTemplateFolder = folder }));
                foreach (var p in ApplicationContext.Setting.Mail.CustomParameters)
                {
                    manager.AddParameter(p.Name, p.Value);
                }
                return manager;
            });

            // Workflow
            app.CreatePerOwinContext<WorkflowManager>((IdentityFactoryOptions<WorkflowManager> options, IOwinContext context) => {
               
                var workflowManager = new WorkflowManager();
                var logger = LogManager.GetLogger("workflow");
                var container = UnityConfig.GetConfiguredContainer();
                var service = container.Resolve<IAccessControlService>();
                var task = container.Resolve<IAcsTask<ExportInterfaceFileToAccessControlTaskOptions>>();
                var mailManager = context.Get<MailManager>();
                workflowManager.AddService(service);
                workflowManager.AddService(mailManager);
                workflowManager.AddService(task);
                workflowManager.RegisterWorkflow(AcsRequestTypeCodes.Employee, typeof(AcsEmployeeWorkflow));
                workflowManager.RegisterWorkflow(AcsRequestTypeCodes.Visitor, typeof(AcsVisitorWorkflow));
                workflowManager.RegisterWorkflow(AcsRequestTypeCodes.ItemIn, typeof(AcsItemInWorkflow));
                workflowManager.RegisterWorkflow(AcsRequestTypeCodes.ItemOut, typeof(AcsItemOutWorkflow));
                workflowManager.RegisterWorkflow(AcsRequestTypeCodes.Photographing, typeof(AcsPhotoWorkflow));

                // Attach Export Task event           
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
                    logger.Info($"Task {task.TaskID}:{task.TaskName} is execute completed. Execute result {(e.Error == null ? "SUCCESS" : "FAILED")}. Total execute {e.TotalTime.TotalMinutes} minutes");
                    if (e.Error != null)
                    {
                        logger.Error($"Task {task.TaskID}:{task.TaskName} is error occured.", e.Error);
                    }
                };

                // Attach Workflow event.
                workflowManager.WorkflowCreated += delegate (object sender, WorkflowCreatedEventArgs e)
                {
                    logger.Info($"Workflow {e.WorkflowInstance.GetType().Name} is created. Description {e.Description}");
                };

                workflowManager.WorkflowStarted += delegate (object sender, WorkflowStartedEventArgs e)
                {
                    logger.Info($"Workflow {e.WorkflowInstance.GetType().Name} is started.");
                };

                workflowManager.WorkflowProgress += delegate (object sender, WorkflowProgressEventArgs e)
                {
                    logger.Info(e.Message);
                };

                workflowManager.WorkflowError += delegate (object sender, WorkflowErrorEventArgs e)
                {
                    logger.Error($"Error occured. Error message: {ExceptionUtility.GetLastExceptionMessage(e.Error)}");
                    logger.Error($"StackTrace: {e.Error.StackTrace}");
                };

                workflowManager.WorkflowCompleted += delegate (object sender, WorkflowCompletedEventArgs e)
                {
                    logger.Info($"Workflow {e.WorkflowInstance.GetType().Name} is execute completed. Total execute {e.TotalUsageTimes.TotalMinutes} minutes");
                };

                return workflowManager;
            });

            app.CreatePerOwinContext<UpdateEmployeeInfoTask>(() => {
                var container = UnityConfig.GetConfiguredContainer();
                var context = container.Resolve<UpdateEmployeeInfoTask>();
                var logger = LogManager.GetLogger(context.TaskID.ToLowerInvariant());

                context.Started += delegate (object sender, EventArgs e)
                {
                    logger.Info($"Task {context.TaskID}:{context.TaskName} is started.");
                };

                context.Progress += delegate (object sender, TaskProgressEventArgs e)
                {
                    logger.Info(e.Message);
                };

                context.Error += delegate (object sender, ErrorEventArgs e)
                {
                    logger.Error($"Error occured. Error message: {ExceptionUtility.GetLastExceptionMessage(e.GetException())}");
                    logger.Error($"StackTrace: {e.GetException().StackTrace}");
                };

                context.Completed += delegate (object sender, TaskCompletedEventArgs e)
                {
                    logger.Info($"Task {context.TaskID}:{context.TaskName} is execute completed. Total execute {e.TotalTime.TotalMinutes} minutes");
                    if (e.Error != null)
                    {
                        logger.Error($"Task {context.TaskID}:{context.TaskName} is error occured.", e.Error);
                    }
                    var user = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                    var service = new AccessControlService();
                    if (e.IsSuccess)
                    {
                        var batchToUpdated = new AcsTask() { TaskID = context.TaskID, LastResultMessage = "The batch task was executed successfully.", UpdateBy = user };
                        service.UpdateAcsTask(batchToUpdated);
                    }
                    else
                    {
                        var message = ExceptionUtility.GetLastExceptionMessage(e.Error);
                        service.UpdateAcsTask(new AcsTask() { TaskID = context.TaskID, LastResultMessage = message, UpdateBy = user, Error = e.Error });
                    }
                };
                return context;
            });

            app.CreatePerOwinContext<ExportInterfaceFileToAccessControlTask>(() => {
                var container = UnityConfig.GetConfiguredContainer();
                var context = container.Resolve<ExportInterfaceFileToAccessControlTask>();
                var logger = LogManager.GetLogger(context.TaskID.ToLowerInvariant());

                context.Started += delegate (object sender, EventArgs e)
                {
                    logger.Info($"Task {context.TaskID}:{context.TaskName} is started.");
                };

                context.Progress += delegate (object sender, TaskProgressEventArgs e)
                {
                    logger.Info(e.Message);
                };

                context.Error += delegate (object sender, ErrorEventArgs e)
                {
                    logger.Error($"Error occured. Error message: {ExceptionUtility.GetLastExceptionMessage(e.GetException())}");
                    logger.Error($"StackTrace: {e.GetException().StackTrace}");
                };

                context.Completed += delegate (object sender, TaskCompletedEventArgs e)
                {
                    logger.Info($"Task {context.TaskID}:{context.TaskName} is execute completed. Total execute {e.TotalTime.TotalMinutes} minutes");
                    if (e.Error != null)
                    {
                        logger.Error($"Task {context.TaskID}:{context.TaskName} is error occured.", e.Error);
                    }
                    var user = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                    var service = new AccessControlService();
                    if (e.IsSuccess)
                    {
                        var batchToUpdated = new AcsTask() { TaskID = context.TaskID, LastResultMessage = "The batch task was executed successfully.", UpdateBy = user };
                        service.UpdateAcsTask(batchToUpdated);
                    }
                    else
                    {
                        var message = ExceptionUtility.GetLastExceptionMessage(e.Error);
                        service.UpdateAcsTask(new AcsTask() { TaskID = context.TaskID, LastResultMessage = message, UpdateBy = user, Error = e.Error });
                    }
                };
                return context;
            });

            var authCookieOptions = new CookieAuthenticationOptions
            {
                ExpireTimeSpan = FormsAuthentication.Timeout,                
                SlidingExpiration = FormsAuthentication.SlidingExpiration,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieName = FormsAuthentication.FormsCookieName,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/LogOut")
            };
            app.UseCookieAuthentication(authCookieOptions);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}
