namespace CSI.ObjectCompare
{
    using System;
    using System.Runtime.CompilerServices;

    public class ObjectCompareResult
    {
        public ObjectCompareResult(object value1, object value2, int result, string breadCrumb, string message)
        {
            this.Value1 = value1;
            this.Value2 = value2;
            this.Result = result;
            this.BreadCrumb = breadCrumb;
            this.Message = this.Message;
        }

        public string BreadCrumb { get; private set; }

        public string Message { get; private set; }

        public int Result { get; private set; }

        public object Value1 { get; private set; }

        public object Value2 { get; private set; }
    }
}

