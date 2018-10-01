namespace CSI.ObjectCompare
{
    using System;

    [Flags]
    public enum ObjectCompareOptions
    {
        All = 7,
        Children = 2,
        Default = 7,
        Fields = 4,
        None = -8,
        PrivateFields = 1,
        PrivateProperties = 0,
        Properties = 5,
        ReadOnlyProperties = 3
    }
}

