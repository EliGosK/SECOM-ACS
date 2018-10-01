using CSI.EPPlus;
using CSI.Localization;
using OfficeOpenXml;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.Style;
using System.Drawing;

namespace SECOM.ACS.Reporting
{
    public class RequestAccessReportBuilder : ExcelReportBuilderBase<RequestAccessReportData>
    {
        protected override void InitializeNamedStyle(ExcelPackage xlPackage)
        {
            base.InitializeNamedStyle(xlPackage);

            var headerStyle = xlPackage.Workbook.Styles.CreateNamedStyle("headerStyle");
            headerStyle.Style.Font.Bold = true;
            headerStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //headerStyle.Style.Fill.PatternColor.SetColor(Color.Gray);
            headerStyle.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(211,217,196));

            var dateStyle = xlPackage.Workbook.Styles.CreateNamedStyle("dateStyle");
            dateStyle.Style.Numberformat.Format = "d/MM/yyyy";
        }

        public override void CreateReport(Stream stream, RequestAccessReportData reportData)
        {
            var rowIndex = 1;
            using (var p = this.CreateExcelPackage())
            {
                var sheet = p.Workbook.Worksheets.Add("Sheet1");

               
                var columnMappings = new List<OutputColumnMapping>();
             
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("DocumentType"), ColumnName = reportData.HeaderNames["DocumentType"] ,Width=32d , HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "ReqNo", ColumnName = reportData.HeaderNames["ReqNo"], Width = 14d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("ReqStatus"), ColumnName = reportData.HeaderNames["ReqStatus"], Width = 16d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryDateFrom", ColumnName = reportData.HeaderNames["EntryDateFrom"], Width = 15d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryTimeFrom", ColumnName = reportData.HeaderNames["EntryTimeFrom"], Width = 15d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryDateTo", ColumnName = reportData.HeaderNames["EntryDateTo"], Width = 15d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryTimeTo", ColumnName = reportData.HeaderNames["EntryTimeTo"], Width = 15d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("RequestBy"), ColumnName = reportData.HeaderNames["RequestBy"] ,Width=22d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = "RequestDate", ColumnName = reportData.HeaderNames["RequestDate"] ,Width=26d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("Area"), ColumnName = reportData.HeaderNames["Area"] ,Width=21d, HeaderStyle = new ColumnItemStyle("headerStyle") });
                WriteHeaderCell(sheet, columnMappings, rowIndex);
                rowIndex++;
                WriteDataCell(sheet, columnMappings, reportData.Data, rowIndex);

                sheet.Cells[1, 1, rowIndex + reportData.Data.Count-1, columnMappings.Count].Style.Border.SetBorder(ExcelBorderPosition.All, ExcelBorderStyle.Thin, Color.Black);
                p.SaveAs(stream);
            }
        }

        protected override void OnExcelRangeWrited(ExcelRange r, OutputColumnMapping columnMapping, object dataItem)
        {
            base.OnExcelRangeWrited(r, columnMapping, dataItem);
            var request = dataItem as RequestDataView;
            switch (columnMapping.DataField)
            {
                case "EntryDateFrom":
                    r.StyleName = "dateStyle";
                   
                    r.Value = request.EntryDateFrom;
                    break;
                case "EntryTimeFrom":
                    if (request.EntryTimeFrom.HasValue)
                    {
                        r.Value = String.Format("{0}:{1:00}", request.EntryTimeFrom.Value.Hours, request.EntryTimeFrom.Value.Minutes);
                    }
                    break;
                case "EntryDateTo":
                    r.StyleName = "dateStyle";

                    r.Value = request.EntryDateTo;
                    break;
                case "EntryTimeTo":
                    if (request.EntryTimeFrom.HasValue)
                    {
                        r.Value = String.Format("{0}:{1:00}", request.EntryTimeTo.Value.Hours, request.EntryTimeFrom.Value.Minutes);
                    }
                    break;
                case "RequestDate":
                    r.StyleName = "dateStyle";

                    r.Value = request.RequestDate;
                    break;
                default:
                    break;
            }
        }

        public override void CreateReport(Stream stream, RequestAccessReportData reportData, string templatePath)
        {
            var rowIndex = 1;
            using (var p = this.CreateExcelPackage(templatePath))
            {
                var sheet = p.Workbook.Worksheets.Add("Sheet1");
                var columnMappings = new List<OutputColumnMapping>();
                columnMappings.Add(new OutputColumnMapping() { RunningNo = true });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("DocumentType"), ColumnName = reportData.HeaderNames["DocumentType"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "ReqNo", ColumnName = reportData.HeaderNames["ReqNo"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("ReqStatus"), ColumnName = reportData.HeaderNames["ReqStatus"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryDateFrom", ColumnName = reportData.HeaderNames["EntryDateFrom"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryTimeFrom", ColumnName = reportData.HeaderNames["EntryTimeFrom"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryDateTo", ColumnName = reportData.HeaderNames["EntryDateTo"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "EntryTimeTo", ColumnName = reportData.HeaderNames["EntryTimeTo"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "RequestBy", ColumnName = reportData.HeaderNames["RequestBy"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = "RequestDate", ColumnName = reportData.HeaderNames["RequestDate"] });
                columnMappings.Add(new OutputColumnMapping() { DataField = ModelLocalizeManager.GetPropertyName<RequestDataView>("Area"), ColumnName = reportData.HeaderNames["Area"] });
                WriteHeaderCell(sheet, columnMappings, rowIndex);
                rowIndex++;
                WriteDataCell(sheet, columnMappings, reportData.Data, rowIndex);

                // Set Border
                sheet.Cells[1, 1, rowIndex, columnMappings.Count].Style.Border.SetBorder(ExcelBorderPosition.All, ExcelBorderStyle.Thin, Color.Black);

                p.SaveAs(stream);
            }
        }
    }

    public class RequestAccessReportData
    {
        public IList<RequestDataView> Data { get; set; }
        public Dictionary<string, string> HeaderNames { get; set; }
    }
}
