using Common.Logging;
using CSI.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SECOM.ACS.Mail;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;
using System;
using System.IO;

namespace SECOM.ACS.Tests.Task
{
    [TestClass]
    public class AcsTaskUnitTest
    {
        [TestMethod]
        public void ACP010_TestMethod()
        {
            var service = new AccessControlService();
            var interfaceService = new DataInterfaceService();
            var task = new UpdateEmployeeInfoTask(service, interfaceService);
            var logger = LogManager.GetLogger(task.TaskID.ToLowerInvariant());            
            AttachTaskEvent(task, logger); 
            var options = new UpdateEmployeeInfoTaskOptions()
            {
                ExportInterfaceFileOptions = new ExportInterfaceFileOptions
                {
                    TemplateFile = @"Shared\acs-import-template.xlsx",                    
                    TargetFolder = @"Shared\acs-export",
                    TargetFileName = "acs-{0:yyyyMMdd-HHmmss_fff}.csv",       
                    HasHeaderRecord = false
                },
                TaskOptions = new UpdateEmployeeInfoOptions() {
                    ImportFile = @"Shared\employee-import\employee.xlsx",
                    BackupFileName = "employee-{0:yyyyMMdd_HHmm}.xlsx",
                    BackupFolder = @"Shared\employee-backup",
                    EnabledBackup = true,
                    User = "Sittichok",
                    UseHeaderRow = true
                }
            };
            task.Execute(options);
        }

        [TestMethod]
        public void ACP020_TestMethod()
        {
            var service = new AccessControlService();
            var documentService = new DocumentService();
            var mailProvider = new RazorMailProvider(new RazorMailOptions() { BaseTemplateFolder = @"C:\Projects\SECOM\ACS\tests\mail-templates" });
            var task = new UpdateDocumentStatusTask(service, documentService, mailProvider);
            var logger = LogManager.GetLogger(task.TaskID.ToLowerInvariant());
            
            AttachTaskEvent(task, logger);
            var options = new UpdateDocumentStatusTaskOptions()
            {
                EnabledNotification = true,
                User = "Sittichok"                
            };
            task.Execute(options);
        }

        [TestMethod]
        public void ACP030_TestMethod()
        {
            var dataInterfaceService = new DataInterfaceService();
            var service = new AccessControlService();
            var task = new ExportInterfaceFileToAccessControlTask(dataInterfaceService, service);
            var logger = LogManager.GetLogger(task.TaskID.ToLowerInvariant());
           
            AttachTaskEvent(task, logger);
            var options = new ExportInterfaceFileToAccessControlTaskOptions()
            {
                TaskOptions = new ExportToAccessControlOptions()
                {
                    ExportModes = ExportToAccessControlModes.Schedule,
                    EffectiveDate = DateTime.Now.Date
                },
                ExportInterfaceFileOptions = new ExportInterfaceFileOptions()
                {
                    TemplateFile = @"Shared\acs-import-template.xlsx",
                    TargetFolder = @"Shared\acs-export",
                    TargetFileName = "acs-{0:yyyyMMdd_HHmm}.xlsx"                    
                }               
            };
            task.Execute(options);
        }

        private void AttachTaskEvent<TOptions>(IAcsTask<TOptions> task,ILog logger)
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
                logger.Info($"Task {task.TaskID}:{task.TaskName} is execute completed. Execute result {(e.Error == null ? "SUCCESS" : "FAILED")}. Total execute {e.TotalTime.TotalMinutes} minutes");
                if (e.Error != null)
                {
                    logger.Error($"Task {task.TaskID}:{task.TaskName} is error occured.", e.Error);
                }
                var user = "sittichok";
                var service = new AccessControlService();
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
