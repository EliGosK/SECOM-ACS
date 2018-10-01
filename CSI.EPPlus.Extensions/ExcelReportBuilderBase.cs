using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CSI.EPPlus
{
    public interface IExcelReportBuilder<T>
        where T : class
    {
        void CreateReport(Stream stream, T dataValues);
    }

    public abstract class ExcelReportBuilderBase<T> : IExcelReportBuilder<T>
        where T : class
    {
        public ExcelReportBuilderBase()
        {
            this.WorkSheetName = "Sheet1";
            InitializeColumnMapping();
        }
        
        public string WorkSheetName { get; set; }
        public List<OutputColumnMapping> ColumnMappings { get; set; }

        protected virtual ExcelPackage CreateExcelPackage()
        {
            ExcelPackage xlPackage = new ExcelPackage();
            InitializeNamedStyle(xlPackage);
            return xlPackage;
        }

        protected virtual ExcelPackage CreateExcelPackage(string templatePath)
        {
            ExcelPackage xlPackage = new ExcelPackage(File.OpenRead(templatePath));
            InitializeNamedStyle(xlPackage);
            return xlPackage;
        }

        protected virtual void InitializeColumnMapping()
        {
            this.ColumnMappings = new List<OutputColumnMapping>();
        }

        protected virtual void InitializeNamedStyle(ExcelPackage xlPackage)
        {

        }

        protected void WriteHeaderCell(ExcelWorksheet sheet, List<OutputColumnMapping> columnMappings, int rowIndex)
        {
            int columnIndex = 1;
            foreach (var columnMap in columnMappings)
            {
                sheet.Cells[rowIndex, columnIndex].Value = columnMap.ColumnName;
                if (columnMap.HeaderStyle != null)
                {
                    sheet.Cells[rowIndex, columnIndex].StyleName = columnMap.HeaderStyle.StyleName;
                }

                if (columnMap.Width > 0)
                {
                    sheet.Column(columnIndex).Width = columnMap.Width;
                }
                columnIndex++;
            }
        }

        protected void WriteHeaderCell(ExcelWorksheet sheet, DataTable dataTable, int rowIndex)
        {
            int columnIndex = 1;
            foreach (DataColumn column in dataTable.Columns)
            {
                sheet.Cells[rowIndex, columnIndex].Value = column.ColumnName;
                columnIndex++;
            }
        }

        protected void WriteDataCell<TData>(ExcelWorksheet sheet, List<OutputColumnMapping> columnMappings, IList<TData> dataItems, int rowIndex)
        {
            int itemNo = 1;
            foreach (var dataItem in dataItems)
            {
                int columnIndex = 1;
                foreach (var columnMap in columnMappings)
                {
                    sheet.Cells[rowIndex, columnIndex].Value = columnMap.RunningNo ? itemNo++ : columnMap.GetValue<TData>(dataItem);
                    sheet.Cells[rowIndex, columnIndex].StyleName = columnMap.ItemStyle.StyleName ?? "Normal";
                    OnExcelRangeWrited(sheet.Cells[rowIndex, columnIndex], columnMap, dataItem);
                    columnIndex++;
                }
                rowIndex++;
            }
        }

        protected void WriteDataCell(ExcelWorksheet sheet, DataTable dataTable, int rowIndex)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    sheet.Cells[rowIndex, columnIndex].Value = row[column];
                    columnIndex++;
                }
                rowIndex++;
            }
        }

        protected void SetBorder(ExcelRange r, System.Drawing.Color color, ExcelBorderPosition borderPositions = ExcelBorderPosition.All, ExcelBorderStyle borderStyle = ExcelBorderStyle.Thin)
        {
            r.Style.Border.SetBorder(borderPositions, borderStyle, color);
        }

        protected void CopyCellValue(ExcelWorksheet sheet, int rowIndex, int columnIndex, int targetRowIndex, int targetCellIndex, CopyCellOptions options = CopyCellOptions.All)
        {
            if ((options & CopyCellOptions.Value) > 0)
            {
                sheet.Cells[targetRowIndex, targetCellIndex].Value = sheet.Cells[rowIndex, columnIndex].Value;
            }

            if ((options & CopyCellOptions.FontStyle) > 0)
            {
                sheet.Cells[targetRowIndex, targetCellIndex].Style.Font.Name = sheet.Cells[rowIndex, columnIndex].Style.Font.Name;
                sheet.Cells[targetRowIndex, targetCellIndex].Style.Font.Bold = sheet.Cells[rowIndex, columnIndex].Style.Font.Bold;
                sheet.Cells[targetRowIndex, targetCellIndex].Style.Font.Size = sheet.Cells[rowIndex, columnIndex].Style.Font.Size;
            }

            if ((options & CopyCellOptions.Fill) > 0)
            {
                sheet.Cells[targetRowIndex, targetCellIndex].Style.Fill = sheet.Cells[rowIndex, columnIndex].Style.Fill;
            }
        }

        protected virtual void OnExcelRangeWrited(ExcelRange r, OutputColumnMapping columnMapping,object dataItem)
        {

        }

        public abstract void CreateReport(Stream stream, T reportData);
        public abstract void CreateReport(Stream stream, T reportData, string templatePath);

    }

    
}
