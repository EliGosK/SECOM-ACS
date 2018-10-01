using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks;

namespace SECOM.ACS.WindowsService
{
    /// <summary>
    /// This represents an entity for the job that actually performs for the schedule.
    /// </summary>
    internal class UpdateDocumentExpirationJob : AcsJobBase<UpdateDocumentStatusTaskOptions>
    {
        
        public UpdateDocumentExpirationJob(IAcsTask<UpdateDocumentStatusTaskOptions> task) : base(task)
        {

        }

        /// <summary>
        /// Called by the <c>Quartz.IScheduler</c> when a <c>Quartz.ITrigger</c> fires that is associated with the <c>Quartz.IJob</c>.
        /// </summary>
        /// <param name="context">JobExecutionContext instance</param>
        public override void Execute(IJobExecutionContext context)
        {
            var options = new UpdateDocumentStatusTaskOptions();
            var dataMap = (string)context.JobDetail.JobDataMap["options"];
            if (!string.IsNullOrEmpty(dataMap))
            {
                options = JsonConvert.DeserializeObject<UpdateDocumentStatusTaskOptions>(dataMap);
            }
            task.Execute(options);
        }
    }
}