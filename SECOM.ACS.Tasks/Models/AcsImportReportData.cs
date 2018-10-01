using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class AcsImportReportData
    {
        public IEnumerable<EmployeeForImportAcs> Employees { get; set; }
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string SheetName { get; set; } = "ACS";

        public string DummyAccessGroup { get; set; } = "DUMMY";
    }
}
