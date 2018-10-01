namespace CSI.ObjectCompare
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class ObjectCompareResultCollection : CollectionBase
    {
        public int Add(ObjectCompareResult item)
        {
            return base.InnerList.Add(item);
        }

        public int Add(object value1, object value2, int result, string breadCrumb, string message)
        {
            return base.InnerList.Add(new ObjectCompareResult(value1, value2, result, breadCrumb, message));
        }

        public ObjectCompareResult this[int index]
        {
            get
            {
                return (base.InnerList[index] as ObjectCompareResult);
            }
        }
    }
}

