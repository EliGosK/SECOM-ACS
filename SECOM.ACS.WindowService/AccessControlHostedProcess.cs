using Atlas;
using Common.Logging;
using CSI.IO;
using Newtonsoft.Json;
using Quartz;
using Quartz.Spi;
using System;
using System.Configuration;

namespace SECOM.ACS.WindowsService
{
    /// <summary>
    /// This represents an entity for Windows Service hosted by Atlas.
    /// </summary>
    internal class AccessControlHostedProcess : IAmAHostedProcess
    {
        private static readonly ILog logger = LogManager.GetLogger("logger");

        /// <summary>
        /// Gets or sets the scheduler instance.
        /// </summary>
        public IScheduler Scheduler { get; set; }           // #1

        /// <summary>
        /// Gets or sets the job factory instance.
        /// </summary>
        public IJobFactory JobFactory { get; set; }         // #2

        /// <summary>
        /// Gets or sets the job listener instance.
        /// </summary>
        public IJobListener JobListener { get; set; }       

        /// <summary>
        /// Starts the Windows Service.
        /// </summary>
        public void Start()
        {
            logger.Info("SECOM Access Control Windows Service is starting");

            var configFile = FileHelper.GetApplicationFullPath(ConfigurationManager.AppSettings["acs.service.config"] ?? "appSetting.json");
            var serviceOptions = AcsScheduleServiceOptions.LoadFromJsonFile(configFile);

            logger.InfoFormat("Load Service Schedule options is success. Options {0}.",JsonConvert.SerializeObject(serviceOptions));

            if (serviceOptions.ImportEmployeeOptions.Enabled)
            {
                var group = "ACP010";
                if (serviceOptions.ImportEmployeeOptions.CronExpressions.Length > 0)
                {
                    // Update Employee Job
                    var index = 1;
                    foreach (var cronExpression in serviceOptions.ImportEmployeeOptions.CronExpressions)
                    {
                        var jobName = $"UpdateEmployeeInfoJob-{index}";
                        var triggerName = $"UpdateEmployeeInfoTrigger-{index}";

                        var job = JobBuilder.Create<UpdateEmployeeInfoJob>()
                                        .WithIdentity(jobName, group)
                                        .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToUpdateEmployeeInfoTaskOptions()))
                                        .Build();

                        var trigger = TriggerBuilder.Create()
                                        .WithIdentity(triggerName, group)
                                        .WithCronSchedule(cronExpression)
                                        .ForJob(jobName, group)
                                        .Build();

                        this.Scheduler.ScheduleJob(job, trigger);
                        index++;
                    }
                }
                else {
                    var jobName = $"UpdateEmployeeInfoJob";
                    var triggerName = $"UpdateEmployeeInfoTrigger";

                    var job = JobBuilder.Create<UpdateEmployeeInfoJob>()
                                    .WithIdentity(jobName, group)
                                    .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToUpdateEmployeeInfoTaskOptions()))
                                    .Build();

                    var trigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerName, group)
                                    .StartNow()
                                    .ForJob(jobName, group)
                                    .Build();

                    this.Scheduler.ScheduleJob(job, trigger);
                }
            }

            if (serviceOptions.UpdateDocumentStatusOptions.Enabled)
            {
                var group = "ACP020";
                // Update Document Expiration Job
                var index = 1;
                if (serviceOptions.UpdateDocumentStatusOptions.CronExpressions.Length > 0)
                {
                    foreach (var cronExpression in serviceOptions.UpdateDocumentStatusOptions.CronExpressions)
                    {
                       
                        var jobName = $"UpdateDocumentExpirationJob-{index}";
                        var triggerName = $"UpdateDocumentExpirationTrigger-{index}";
                        var job = JobBuilder.Create<UpdateDocumentExpirationJob>()
                                    .WithIdentity(jobName, group)
                                    .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToUpdateDocumentStatusOptions()))
                                    .Build();

                        var trigger = TriggerBuilder.Create()
                                        .WithIdentity(triggerName, group)
                                        .WithCronSchedule(cronExpression)
                                        .ForJob(jobName, group)
                                        .Build();

                        this.Scheduler.ScheduleJob(job, trigger);
                        index++;
                    }
                }
                else {
                    var jobName = $"UpdateDocumentExpirationJob";
                    var triggerName = $"UpdateDocumentExpirationTrigger";
                    var job = JobBuilder.Create<UpdateDocumentExpirationJob>()
                                .WithIdentity(jobName, group)
                                .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToUpdateDocumentStatusOptions()))
                                .Build();

                    var trigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerName, group)
                                    .StartNow()
                                    .ForJob(jobName, group)
                                    .Build();
                }
            }

            if (serviceOptions.ExportInterfaceFileToAccessControlOptions.Enabled)
            {
                var group = "ACP030";
                var taskOptons = serviceOptions.ToExportToAccessControlTaskOptions();
                taskOptons.TaskOptions.EffectiveDate = DateTime.Now.Date;
                // Export To Access Control Job
                if (serviceOptions.ExportInterfaceFileToAccessControlOptions.CronExpressions.Length > 0)
                { 
                    var index = 1;
                    foreach (var cronExpression in serviceOptions.ExportInterfaceFileToAccessControlOptions.CronExpressions)
                    {
                        
                        var jobName = $"ExportToAccessControlJob-{index}";
                        var triggerName = $"ExportToAccessControlTrigger-{index}";
                        var job = JobBuilder.Create<ExportToAccessControlJob>()
                                      .WithIdentity(jobName, group)
                                      .UsingJobData("options", JsonConvert.SerializeObject(taskOptons))
                                      .Build();

                        var trigger = TriggerBuilder.Create()
                                            .WithIdentity(triggerName, group)
                                            .WithCronSchedule(cronExpression)
                                            .ForJob(jobName, group)
                                            .Build();

                        this.Scheduler.ScheduleJob(job, trigger);
                        index++;
                    }
                }
                else
                {
                    var jobName = $"ExportToAccessControlJob";
                    var triggerName = $"ExportToAccessControlTrigger";
                    var job = JobBuilder.Create<ExportToAccessControlJob>()
                                  .WithIdentity(jobName, group)
                                  .UsingJobData("options", JsonConvert.SerializeObject(taskOptons))
                                  .Build();

                    var trigger = TriggerBuilder.Create()
                                        .WithIdentity(triggerName, group)
                                        .StartNow()
                                        .ForJob(jobName, group)
                                        .Build();

                    this.Scheduler.ScheduleJob(job, trigger);
                }
            }

            if (serviceOptions.TransferInterfaceFileToAccessControlOptions.Enabled)
            {
                var group = "ACP040";
                // Export To Access Control Job
                if (serviceOptions.TransferInterfaceFileToAccessControlOptions.CronExpressions.Length > 0)
                {
                    var index = 1;
                    foreach (var cronExpression in serviceOptions.TransferInterfaceFileToAccessControlOptions.CronExpressions)
                    {
                        var jobName = $"ImportToAccessControlJob-{index}";
                        var triggerName = $"ImportToAccessControlTrigger-{index}";
                        var job = JobBuilder.Create<ImportToAccessControlJob>()
                                      .WithIdentity(jobName, group)
                                      .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToImportToAccessControlTaskOptions()))
                                      .Build();

                        var trigger = TriggerBuilder.Create()
                                            .WithIdentity(triggerName, group)
                                            .WithCronSchedule(cronExpression)
                                            .ForJob(jobName, group)
                                            .Build();

                        this.Scheduler.ScheduleJob(job, trigger);
                        index++;
                    }
                }
                else
                {
                    var jobName = $"ImportToAccessControlJob";
                    var triggerName = $"ImportToAccessControlTrigger";
                    var job = JobBuilder.Create<ImportToAccessControlJob>()
                                  .WithIdentity(jobName, group)
                                  .UsingJobData("options", JsonConvert.SerializeObject(serviceOptions.ToImportToAccessControlTaskOptions()))
                                  .Build();

                    var trigger = TriggerBuilder.Create()
                                        .WithIdentity(triggerName, group)
                                        .StartNow()
                                        .ForJob(jobName, group)
                                        .Build();

                    this.Scheduler.ScheduleJob(job, trigger);
                }
            }
            this.Scheduler.JobFactory = this.JobFactory;    
            this.Scheduler.ListenerManager.AddJobListener(this.JobListener);    
            this.Scheduler.Start();                         

            logger.Info("SECOM Access Control Windows Service is started");
        }

        /// <summary>
        /// Stops the Windows Service.
        /// </summary>
        public void Stop()
        {
            logger.Info("SECOM Access Control Windows Service stopping");

            this.Scheduler.Shutdown();

            logger.Info("SECOM Access Control Windows Service stopped");
        }

        /// <summary>
        /// Resumes the Windows Service.
        /// </summary>
        public void Resume()
        {
            logger.Info("SECOM Access Control Windows Service resuming");

            this.Scheduler.ResumeAll();

            logger.Info("SECOM Access Control Windows Service resumed");
        }

        /// <summary>
        /// Pauses the Windows Service.
        /// </summary>
        public void Pause()
        {
            logger.Info("SECOM Access Control Windows Service pausing");

            this.Scheduler.PauseAll();

            logger.Info("SECOM Access Control Windows Service paused");
        }
    }
}