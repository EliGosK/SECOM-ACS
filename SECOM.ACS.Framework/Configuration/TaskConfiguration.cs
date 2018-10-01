using SECOM.ACS.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class TaskConfiguration
    {
        public ExportInterfaceFileOptions ExportFileOptions { get; set; } = new ExportInterfaceFileOptions();
        public UpdateEmployeeInfoOptions ImportEmployeeOptions { get; set; } = new UpdateEmployeeInfoOptions();
        public UpdateDocumentExpirationOptions UpdateDocumentExpirationOptions { get; set; } = new UpdateDocumentStatusTaskOptions();
        public ExportToAccessControlOptions ExportToAccessControlOptions { get; set; } = new ExportToAccessControlOptions();

        public UpdateEmployeeInfoTaskOptions ToUpdateEmployeeInfoTaskOptions()
        {
            return new UpdateEmployeeInfoTaskOptions()
            {
                ExportInterfaceFileOptions= this.ExportFileOptions,
                TaskOptions = this.ImportEmployeeOptions
            };
        }

        public UpdateDocumentStatusTaskOptions ToUpdateDocumentExpirationOptions()
        {
            return new UpdateDocumentStatusTaskOptions()
            {
                DocumentTypes = this.UpdateDocumentExpirationOptions.DocumentTypes,
                EnabledNotification = this.UpdateDocumentExpirationOptions.EnabledNotification,
                User = this.UpdateDocumentExpirationOptions.User
            };
        }

        public ExportInterfaceFileToAccessControlTaskOptions ToExportToAccessControlTaskOptions()
        {
            return new ExportInterfaceFileToAccessControlTaskOptions()
            {
                ExportInterfaceFileOptions = this.ExportFileOptions,
                TaskOptions = this.ExportToAccessControlOptions
            };
        }
    }
    
}
