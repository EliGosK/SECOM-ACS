using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class ExportInterfaceFileToAccessControlTaskOptions : IExportInterfaceFileTaskOptions
    {
        public ExportToAccessControlOptions TaskOptions { get; set; } = new ExportToAccessControlOptions();
        public ExportInterfaceFileOptions ExportInterfaceFileOptions { get; set; } = new ExportInterfaceFileOptions();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExportToAccessControlOptions 
    {
        public ExportToAccessControlModes ExportModes { get; set; } = ExportToAccessControlModes.Add;
        [JsonIgnore]
        public Guid[] Transactions { get; set; }
        [JsonIgnore]
        public DateTime EffectiveDate { get; set; } = DateTime.Now.Date;
        [JsonIgnore]
        public string[] Employees { get; set; }

    }

    public enum ExportToAccessControlModes
    {
        Add = 1,
        Cancel = 2,
        Schedule = 4 ,
        Employee = 8
    }

    
}
