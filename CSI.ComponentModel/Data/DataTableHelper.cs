using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CSI.Data
{
    public class DataTableHelper
    {
        private bool ColumnEqual(object A, object B)
        {
            if (object.ReferenceEquals(A, DBNull.Value) & object.ReferenceEquals(B, DBNull.Value))
            {
                return true;
            }
            if (object.ReferenceEquals(A, DBNull.Value) | object.ReferenceEquals(B, DBNull.Value))
            {
                return false;
            }
            if (A == null)
            {
                return B.Equals(A);
            }
            return A.Equals(B);
        }

        private bool CompareDataColumnValues(DataRow row, DataRow row2, string[] columnNames)
        {
            new StringBuilder();
            foreach (string str in columnNames)
            {
                if (row[str] != row2[str])
                {
                    return false;
                }
            }
            return true;
        }

        public void FillDistinct(DataTable targetTable, DataRow[] sourceDataRows, string[] columnNames)
        {
            DataRow row = null;
            foreach (DataRow row2 in sourceDataRows)
            {
                if ((row == null) || !this.CompareDataColumnValues(row, row2, columnNames))
                {
                    row = row2;
                    targetTable.ImportRow(row2);
                }
            }
        }

        public DataTable GetDistinct(DataTable sourceTable, DataColumn[] columns)
        {
            List<string> list = new List<string>(columns.Length);
            foreach (DataColumn column in columns)
            {
                list.Add(column.ColumnName);
            }
            return this.GetDistinct(sourceTable, list.ToArray());
        }

        public DataTable GetDistinct(DataTable sourceTable, string[] columnNames)
        {
            return this.GetDistinct(sourceTable, columnNames, string.Join(",", columnNames));
        }

        public DataTable GetDistinct(DataTable sourceTable, DataColumn[] columns, string sortBy)
        {
            List<string> list = new List<string>(columns.Length);
            foreach (DataColumn column in columns)
            {
                list.Add(column.ColumnName);
            }
            return this.GetDistinct(sourceTable, list.ToArray(), sortBy);
        }

        public DataTable GetDistinct(DataTable sourceTable, string[] columnNames, string sortBy)
        {
            DataRow row = null;
            DataTable table = new DataTable(string.Format("{0}_Distinct", sourceTable.TableName));
            table = sourceTable.Clone();
            foreach (DataRow row2 in sourceTable.Select("", sortBy))
            {
                if ((row == null) || !this.CompareDataColumnValues(row, row2, columnNames))
                {
                    row = row2;
                    table.ImportRow(row2);
                }
            }
            return table;
        }

        public DataRow[] GetDistinctRows(DataRow[] sourceDataRows, string[] columnNames)
        {
            List<DataRow> list = new List<DataRow>();
            DataRow row = null;
            foreach (DataRow row2 in sourceDataRows)
            {
                if ((row == null) || !this.CompareDataColumnValues(row, row2, columnNames))
                {
                    row = row2;
                    list.Add(row2);
                }
            }
            return list.ToArray();
        }

        public DataRow[] SelectDistinct(DataTable sourceTable, string[] columnNames)
        {
            return this.SelectDistinct(sourceTable, columnNames, string.Empty);
        }

        public DataRow[] SelectDistinct(DataTable sourceTable, string[] columnNames, string sortBy)
        {
            DataRow row = null;
            List<DataRow> list = new List<DataRow>();
            foreach (DataRow row2 in sourceTable.Select("", sortBy))
            {
                if ((row == null) || !this.CompareDataColumnValues(row, row2, columnNames))
                {
                    row = row2;
                    list.Add(row2);
                }
            }
            return list.ToArray();
        }
    }
}

