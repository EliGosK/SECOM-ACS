namespace CSI.Enumerations
{
    using System;
    using System.Collections.Generic;

    public class EnumItemComparer : IComparer<EnumItem>
    {
        public int Compare(EnumItem x, EnumItem y)
        {
            if (x.Order > y.Order)
            {
                return 1;
            }
            if (x.Order != y.Order)
            {
                return -1;
            }
            if (x.Value > y.Value)
            {
                return 1;
            }
            if (x.Value != y.Value)
            {
                return -1;
            }
            return 0;
        }
    }
}

