using Newtonsoft.Json;
using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.WindowsService
{
    public class AcsScheduleServiceOptions
    {
        public ExportInterfaceFileOptions ExportFileOptions { get; set; } = new ExportInterfaceFileOptions();
        public ImportEmployeeScheduleServiceOptions ImportEmployeeOptions { get; set; } = new ImportEmployeeScheduleServiceOptions();
        public UpdateDocumentStatusScheduleServiceOptions UpdateDocumentStatusOptions { get; set; } = new UpdateDocumentStatusScheduleServiceOptions();
        public ExportInterfaceFileForAccessControlScheduleServiceOptions ExportInterfaceFileToAccessControlOptions { get; set; } = new ExportInterfaceFileForAccessControlScheduleServiceOptions();
        public TransferInterfaceFileToAccessControlScheduleServiceOptions TransferInterfaceFileToAccessControlOptions { get; set; } = new TransferInterfaceFileToAccessControlScheduleServiceOptions();
        public string EmailTemplateFolder { get; set; } = "EmailTemplates";
      

        public static AcsScheduleServiceOptions LoadFromJsonFile(string file)
        {
            using (var sr = File.OpenText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize(sr, typeof(AcsScheduleServiceOptions)) as AcsScheduleServiceOptions;
            }
        }

        public static void SaveConfiguration(string file, AcsScheduleServiceOptions options)
        {
            using (StreamWriter sw = File.CreateText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(sw, options);
            }
        }

        public UpdateEmployeeInfoTaskOptions ToUpdateEmployeeInfoTaskOptions()
        {
            return new UpdateEmployeeInfoTaskOptions()
            {
                ExportInterfaceFileOptions = this.ExportFileOptions,
                TaskOptions = ImportEmployeeOptions
            };
        }

        public UpdateDocumentStatusTaskOptions ToUpdateDocumentStatusOptions()
        {
            return new UpdateDocumentStatusTaskOptions()
            {
                DocumentTypes = this.UpdateDocumentStatusOptions.DocumentTypes,
                EnabledNotification = this.UpdateDocumentStatusOptions.EnabledNotification,
                User = this.UpdateDocumentStatusOptions.User
            }; 
        }

        public ExportInterfaceFileToAccessControlTaskOptions ToExportToAccessControlTaskOptions()
        {
            return new ExportInterfaceFileToAccessControlTaskOptions()
            {
                ExportInterfaceFileOptions = this.ExportFileOptions,
                TaskOptions = this.ExportInterfaceFileToAccessControlOptions
            };
        }

        public TransferInterfaceFileToAccessControlTaskOptions ToImportToAccessControlTaskOptions()
        {
            return new TransferInterfaceFileToAccessControlTaskOptions()
            {
                SourceFolder = this.TransferInterfaceFileToAccessControlOptions.SourceFolder,

                TargetFileName = this.TransferInterfaceFileToAccessControlOptions.TargetFileName,
                TargetFolder = this.TransferInterfaceFileToAccessControlOptions.TargetFolder,

                EnabledArchive = this.TransferInterfaceFileToAccessControlOptions.EnabledArchive,
                ArchiveFolder = this.TransferInterfaceFileToAccessControlOptions.ArchiveFolder,

                EnabledNetworkShareAuthenticate = this.TransferInterfaceFileToAccessControlOptions.EnabledNetworkShareAuthenticate,
                RemoteComputerName = this.TransferInterfaceFileToAccessControlOptions.RemoteComputerName,
                UserName = this.TransferInterfaceFileToAccessControlOptions.UserName,
                Password = this.TransferInterfaceFileToAccessControlOptions.Password
            };
        }
    }

    public class ImportEmployeeScheduleServiceOptions : UpdateEmployeeInfoOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] CronExpressions { get; set; } = new string[] { "0 0 8 * * ? *" };
        public bool Enabled { get; set; } = true;
    }

   

    public class UpdateDocumentStatusScheduleServiceOptions : UpdateDocumentExpirationOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] CronExpressions { get; set; } = new string[] { "0 0 8 * * ? *" };
        public bool Enabled { get; set; } = true;
    }

    public class ExportInterfaceFileForAccessControlScheduleServiceOptions : ExportToAccessControlOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] CronExpressions { get; set; } = new string[] { "0 0/5 * * * ? *" };

        public bool Enabled { get; set; } = true;
    }

    public class TransferInterfaceFileToAccessControlScheduleServiceOptions : ImportToAccessControlOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] CronExpressions { get; set; } = new string[] { "0 0 8 * * ? *" };

        public bool Enabled { get; set; } = true;
    }
}
