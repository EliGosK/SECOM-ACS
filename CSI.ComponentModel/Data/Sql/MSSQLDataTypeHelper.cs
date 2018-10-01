using System;

namespace CSI.Data.Sql
{

    public static class MSSQLDataTypeHelper
    {
        public static DateTime GetMaxDateTime()
        {
            return DateTime.MaxValue;
        }

        public static DateTime GetMaxDateTime2()
        {
            return DateTime.MaxValue;
        }

        public static DateTime GetMinDateTime()
        {
            return new DateTime(1900, 1, 1);
        }

        public static DateTime GetMinDateTime2()
        {
            return DateTime.MinValue;
        }
    }
}

