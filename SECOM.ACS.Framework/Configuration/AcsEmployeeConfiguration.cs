using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class AcsEmployeeConfiguration
    {
        public string[] AllowedFileExtensions { get; set; } = new string[] { ".csv" };
        public long AllowUploadFileMaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB

        public int EntryPeriodMonths { get; set; } = 1;
    }

}
