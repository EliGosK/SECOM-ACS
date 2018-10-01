using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class UpdateEmployeeInfoTaskOptions : IExportInterfaceFileTaskOptions
    {
        public ExportInterfaceFileOptions ExportInterfaceFileOptions { get; set; } = new ExportInterfaceFileOptions();
        public UpdateEmployeeInfoOptions TaskOptions { get; set; } = new UpdateEmployeeInfoOptions();
    }

    public class UpdateEmployeeInfoOptions 
    {
        /// <summary>
        /// Get or set employee import file
        /// </summary>
        public string ImportFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public EmployeeImportData[] EmployeeToImports { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool UseHeaderRow { get; set; } = true;
        /// <summary>
        /// 
        /// </summary>
        public string User { get; set; } = "ACP010";

        /// <summary>
        /// Get or set enabled back up import employee file.
        /// </summary>
        public bool EnabledBackup { get; set; } = false;

        /// <summary>
        /// Get or set back up employee import file name
        /// </summary>
        public string BackupFileName { get; set; } = "acs-backup-{0:yyyyMMdd_HHmm}.xls";
        /// <summary>
        /// Get or set back up employee import directory
        /// </summary>
        public string BackupFolder { get; set; } = "backup";


        /// <summary>
        /// Get or set enabled back up import employee file.
        /// </summary>
        public bool EnabledCreateErrorFile { get; set; } = false;
        /// <summary>
        /// Get or set directory that storage excel file that error ocurred
        /// </summary>
        public string ErrorFileName { get; set; } = "employee-{0:yyyyMMdd_HHmm}.xls";
        /// <summary>
        /// Get or set directory that storage excel file that error ocurred
        /// </summary>
        public string ErrorFolder { get; set; } = "employee-import-error";


        /// <summary>
        /// Get or set enabled send update employee information task mail to acknowledge
        /// </summary>
        public bool EnabledSendResultMail { get; set; } = false;
        /// <summary>
        /// Get or set email address to who responsible update employee information
        /// </summary>
        public string[] MailTo { get; set; }

    }
}
