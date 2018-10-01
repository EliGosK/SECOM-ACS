using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SECOM.ACS.Tasks;
using SECOM.ACS.Models;
using System.Collections.Generic;
using CSI.Text;
using SECOM.ACS.Services;
using System.Security.Cryptography;

namespace SECOM.ACS.Tests.Task
{
    [TestClass]
    public class AcsInterfaceFileBuilderUnitTest
    {
        [TestMethod]
        public void AcsImportFileBuilder_Excel_TestMethod()
        {
            var builder = new AcsInterfacetFileBuilder();
            var employees = SeedEmployee();
            var outputFile = @"Shared\acs.xlsx";
            var templateFile = @"Shared\acs-import-template.xlsx";
            var reportData = new AcsImportReportData()
            {
                Employees = employees
            };
            builder.CreateExcelReport(reportData, outputFile, templateFile);
        }

        [TestMethod]
        public void AcsImportFileBuilder_Csv_TestMethod()
        {
            var builder = new AcsInterfacetFileBuilder();
            var employees = SeedEmployee();
            var outputFile = @"Shared\acs.csv";
            var options = new AcsCsvConfiguration() {
                HasHeaderRecord = false
            };
            builder.CreateCsvReport(employees, outputFile,options);
        }

        private IEnumerable<EmployeeForImportAcs> SeedEmployee()
        {
            var employeeForExports = new List<EmployeeForImportAcs>();
            var service = new AccessControlService();
            var employees = service.GetAllEmployee();
            var rnd = TextGenerator.GetRandom();

            foreach (var employee in employees)
            {
                var spaceIndex = employee.EmpNameEN.IndexOf(' ');
                var employeeForExport = new EmployeeForImportAcs()
                {
                    EmpID = TextGenerator.GenerateNumber(6, 8),
                    CardNo = TextGenerator.GenerateNumber(10),
                    Company = "CSI",
                    Position = "PG",
                    Department = "BG-EXP",
                    CardFormat = "SECOM",
                    FirstName = spaceIndex < 0 ? employee.EmpNameEN : employee.EmpNameEN.Substring(0, spaceIndex),
                    LastName = spaceIndex < 0 ?  "":employee.EmpNameEN.Substring(employee.EmpNameEN.IndexOf(' ') + 1)
                };
                  
                var group = rnd.Next(1, 50);
                for (int i = 1; i <= group; i++)
                {
                    var offsetDays = rnd.Next(0, 30);
                    var addDays = rnd.Next(1, 30);
                    var startDate = DateTime.Now.AddDays(offsetDays);
                    var s = typeof(EmployeeForImportAcs).GetProperty($"StartGroup{i}");
                    s.SetValue(employeeForExport, startDate);
                    var e = typeof(EmployeeForImportAcs).GetProperty($"ExpireGroup{i}");
                    e.SetValue(employeeForExport, startDate.AddDays(addDays));
                    var a = typeof(EmployeeForImportAcs).GetProperty($"AccessGroup{i}");
                    a.SetValue(employeeForExport, TextGenerator.GenerateUpperCharacter(1));
                }
                employeeForExports.Add(employeeForExport);
            }
            return employeeForExports;
        }

        private IEnumerable<EmployeeForImportAcs> SeedEmployee2()
        {
            var employeeForExports = new List<EmployeeForImportAcs>();
            var rnd = new Random();
            var totalRecords = rnd.Next(10, 100);
           
            for (int rowIndex = 0; rowIndex <= totalRecords; rowIndex++)
            {
                var employeeForExport = new EmployeeForImportAcs()
                {
                    EmpID = TextGenerator.GenerateNumber(10,10),
                    CardNo = TextGenerator.GenerateNumber(16, 16),
                    Company = "CSI",
                    Department = "BG-EXP",
                    CardFormat = "SECOM"
                };
                var group = rnd.Next(1, 50);
                for (int i = 1; i <= group; i++)
                {
                    var offsetDays = rnd.Next(0, 30);
                    var addDays = rnd.Next(1, 30);
                    var startDate = DateTime.Now.AddDays(offsetDays);
                    var s = typeof(EmployeeForImportAcs).GetProperty($"StartGroup{i}");
                    s.SetValue(employeeForExport, startDate.ToShortDateString());
                    var e = typeof(EmployeeForImportAcs).GetProperty($"ExpireGroup{i}");
                    e.SetValue(employeeForExport, startDate.AddDays(addDays).ToShortDateString());
                    var a = typeof(EmployeeForImportAcs).GetProperty($"AccessGroup{i}");
                    a.SetValue(employeeForExport, TextGenerator.GenerateUpperCharacter(1));
                }
                employeeForExports.Add(employeeForExport);
            }
            return employeeForExports;
        }
    }
}
