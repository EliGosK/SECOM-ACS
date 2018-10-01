namespace CSI.ObjectCompare
{
    using System;
    using System.Data;

    public class TypeHelper
    {
        public static bool IsArray(Type t)
        {
            return t.IsArray;
        }

        public static bool IsChildType(Type t)
        {
            if (IsSimpleType(t))
            {
                return false;
            }
            if ((!IsClass(t) && !IsArray(t)) && (!IsIDictionary(t) && !IsIList(t)))
            {
                return IsStruct(t);
            }
            return true;
        }

        public static bool IsClass(Type t)
        {
            return t.IsClass;
        }

        public static bool IsDataRow(Type t)
        {
            return (t == typeof(DataRow));
        }

        public static bool IsDataset(Type t)
        {
            return (t == typeof(DataSet));
        }

        public static bool IsDataTable(Type t)
        {
            return (t == typeof(DataTable));
        }

        public static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        public static bool IsIDictionary(Type t)
        {
            return (t.GetInterface("System.Collections.IDictionary", true) != null);
        }

        public static bool IsIList(Type t)
        {
            return (t.GetInterface("System.Collections.IList", true) != null);
        }

        public static bool IsSimpleType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                t = Nullable.GetUnderlyingType(t);
            }
            if ((!t.IsPrimitive && (t != typeof(DateTime))) && ((t != typeof(decimal)) && (t != typeof(string))))
            {
                return (t == typeof(Guid));
            }
            return true;
        }

        public static bool IsStruct(Type t)
        {
            return (t.IsValueType && !IsSimpleType(t));
        }

        public static bool IsTimespan(Type t)
        {
            return (t == typeof(TimeSpan));
        }

        public static bool ValidStructSubType(Type t)
        {
            if (((!IsSimpleType(t) && !IsEnum(t)) && (!IsArray(t) && !IsClass(t))) && (!IsIDictionary(t) && !IsTimespan(t)))
            {
                return IsIList(t);
            }
            return true;
        }
    }
}

