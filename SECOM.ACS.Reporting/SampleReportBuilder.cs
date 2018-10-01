using CSI.EPPlus;
using OfficeOpenXml;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Reporting
{
    public class SampleReportBuilder : ExcelReportBuilderBase<SampleReportData>
    {
        public override void CreateReport(Stream stream, SampleReportData reportData)
        {
            var rowIndex = 1;
            using (var p = this.CreateExcelPackage())
            {
                p.Workbook.Worksheets.Add("Sheet1");
                var sheet = p.Workbook.Worksheets[1];

                var columnMappings = new List<OutputColumnMapping>();
                columnMappings.Add(OutputColumnMapping.CreateRunningNoColumn("No.",10d,new ColumnItemStyle("No")));
                columnMappings.Add(OutputColumnMapping.Create("Request No.","ReqNo", 12d,new ColumnItemStyle("RequestNo")));
                columnMappings.Add(OutputColumnMapping.Create("Area", "AreaEN", 12d, new ColumnItemStyle("Area")));
                columnMappings.Add(OutputColumnMapping.Create("Object ID", "ObjectID", 12d, new ColumnItemStyle("ObjectID")));

                WriteHeaderCell(sheet, columnMappings, rowIndex);
                rowIndex++;
                WriteDataCell(sheet, columnMappings, reportData.Data, rowIndex);
                p.SaveAs(stream);
            }
        }

        public override void CreateReport(Stream stream, SampleReportData reportData, string templatePath)
        {
            throw new NotImplementedException();
        }
    }

    public class SampleReportData
    {
        public IList<SampleData> Data { get; set; }
        public Dictionary<string,string> HeaderNames { get; set; }
    }

    public class SampleData
    {
        public string ReqNo { get; set; }
        public string AreaEN { get; set; }
        public string ObjectID { get; set; }
    }
}
