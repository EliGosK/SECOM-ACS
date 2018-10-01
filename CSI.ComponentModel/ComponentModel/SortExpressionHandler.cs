namespace CSI.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using CSI.ComponentModel.Sorting;

    public class SortExpressionHandler
    {
        public static SortExpression[] Parse(string expression)
        {
            List<SortExpression> list = new List<SortExpression>();
            foreach (string str in expression.Split(new char[] { ',' }))
            {
                string[] strArray2 = str.Split(new char[] { ' ' });
                switch (strArray2.Length)
                {
                    case 1:
                        list.Add(new SortExpression(strArray2[0]));
                        break;

                    case 2:
                    {
                        SortingType sortType = (string.Compare(strArray2[1], "DESC", true) == 0) ? SortingType.Descending : SortingType.Ascending;
                        list.Add(new SortExpression(strArray2[0], sortType));
                        break;
                    }
                }
            }
            return list.ToArray();
        }
    }
}

