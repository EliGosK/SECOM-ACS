using System;
using System.Reflection;
using CSI.ComponentModel.Sorting;

namespace CSI.ComponentModel
{
    public class ObjectComparerHelper
    {
        public static int Compare<T>(T value1, T value2, string expressions) where T : class
        {
            return Compare<T>(value1, value2, SortExpressionHandler.Parse(expressions));
        }

        public static int Compare<T>(T value1, T value2, SortExpression[] expressions) where T: class
        {
            if (value1 is IComparable<T>)
            {
                return ((IComparable<T>) value1).CompareTo(value2);
            }
            int num = 0;
            foreach (SortExpression expression in expressions)
            {
                PropertyInfo property = typeof(T).GetProperty(expression.FieldName);
                object obj2 = property.GetValue(value1, null);
                object obj3 = property.GetValue(value2, null);
                if (property.PropertyType == typeof(string))
                {
                    num = string.Compare((string) obj2, (string) obj3);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    if (((bool) obj2) == ((bool) obj3))
                    {
                        num = 0;
                    }
                    else
                    {
                        num = ((bool) obj2) ? 1 : -1;
                    }
                }
                else if (property.PropertyType == typeof(short))
                {
                    num = ((short) obj2) - ((short) obj3);
                }
                else if (property.PropertyType == typeof(int))
                {
                    num = ((int) obj2) - ((int) obj3);
                }
                else if (property.PropertyType == typeof(long))
                {
                    if (((long) obj2) == ((long) obj3))
                    {
                        num = 0;
                    }
                    else
                    {
                        num = (((long) obj2) < ((long) obj3)) ? -1 : 1;
                    }
                }
                else if (property.PropertyType == typeof(double))
                {
                    if (((double) obj2) == ((double) obj3))
                    {
                        num = 0;
                    }
                    else
                    {
                        num = (((double) obj2) < ((double) obj3)) ? -1 : 1;
                    }
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    num = DateTime.Compare((DateTime) obj2, (DateTime) obj3);
                }
                else if (property.PropertyType == typeof(TimeSpan))
                {
                    num = TimeSpan.Compare((TimeSpan) obj2, (TimeSpan) obj3);
                }
                else
                {
                    num = obj2.GetHashCode() - obj3.GetHashCode();
                }
                if (num != 0)
                {
                    if (expression.SortType == SortingType.Descending)
                    {
                        num *= -1;
                    }
                    return num;
                }
            }
            return num;
        }
    }
}

