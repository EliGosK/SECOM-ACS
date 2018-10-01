namespace CSI.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;
    using CSI.ComponentModel.Sorting;

    public class SortExpression
    {
        public SortExpression(string fieldName) : this(fieldName, SortingType.Ascending)
        {
        }

        public SortExpression(string fieldName, SortingType sortType)
        {
            this.FieldName = fieldName;
            this.SortType = sortType;
        }

        public string FieldName { get; set; }

        public SortingType SortType { get; set; }
    }
}

