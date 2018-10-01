using CSI.EPPlus;
using CsvHelper;
using CsvHelper.Configuration;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using SECOM.ACS.Tasks.ClassMaps;

namespace SECOM.ACS.Tasks
{
    public class AcsInterfacetFileBuilder : ExcelReportBuilderBase<AcsImportReportData>
    {
        public override void CreateReport(Stream stream, AcsImportReportData reportData)
        {
            throw new NotImplementedException();
        }

        public override void CreateReport(Stream stream, AcsImportReportData reportData, string templatePath)
        {
            throw new NotImplementedException();
        }

        public void CreateExcelReport(AcsImportReportData reportData, string outputFile, string templatePath)
        {
            var rowIndex = 2;
            using (var p = this.CreateExcelPackage(templatePath))
            {
                var sheet = p.Workbook.Worksheets[1];
                sheet.Name = reportData.SheetName;

                var dummyStartGroup = DateTime.Today.AddDays(-1);
                var dummyExpireGroup = DateTime.Today.AddYears(60);
                foreach (var dataItem in reportData.Employees)
                {
                    var columnIndex = 1;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.EmpID;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.CardNo;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.CardFormat;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.FirstName;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.LastName;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.Company;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.Department;
                    sheet.Cells[rowIndex, columnIndex++].Value = dataItem.Position;

                    for (int i = 1; i <= 50; i++)
                    {
                        var a = typeof(EmployeeForImportAcs).GetProperty($"AccessGroup{i}");
                        var s = typeof(EmployeeForImportAcs).GetProperty($"StartGroup{i}");
                        var e = typeof(EmployeeForImportAcs).GetProperty($"ExpireGroup{i}");

                        sheet.Cells[rowIndex, columnIndex++].Value = a == null ? null : a.GetValue(dataItem, null)?? reportData.DummyAccessGroup;
                        sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = reportData.DateFormat;
                        sheet.Cells[rowIndex, columnIndex++].Value = s == null ? null : s.GetValue(dataItem, null)?? dummyStartGroup;
                        sheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = reportData.DateFormat;
                        sheet.Cells[rowIndex, columnIndex++].Value = e == null ? null : e.GetValue(dataItem, null)?? dummyExpireGroup;
                    }
                    sheet.Cells[rowIndex, columnIndex].Value = "Root Division";
                    rowIndex++;
                }
                p.SaveAs(new FileInfo(outputFile));
            }
        }

        public void CreateCsvReport(IEnumerable<EmployeeForImportAcs> employees, string outputFile, AcsCsvConfiguration options)
        {
            using (var textWriter = new StreamWriter(File.OpenWrite(outputFile)))
            {
                var configuration = new CsvConfiguration() { HasHeaderRecord = options.HasHeaderRecord, Delimiter = options.Delimiter };
                configuration.RegisterClassMap(new EmployeeForImportAcsMap(options.DummyAccessGroup, options.DateFormat));
                using (var writer = new CsvWriter(textWriter, configuration))
                {
                    writer.WriteRecords(employees);
                }
            }
        }
    }

   

    
  
}
