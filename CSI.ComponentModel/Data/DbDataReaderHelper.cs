using System;
using System.Reflection;
using System.Data.Common;

namespace CSI.Data
{
    public class DbDataReaderHelper
    {
        public static T Get<T, TReader>(TReader reader, int column, T nullValue) where TReader: DbDataReader
        {
            if (reader.IsDBNull(column))
            {
                return nullValue;
            }
            string name = string.Format("Get{0}", typeof(T).Name);
            MethodInfo method = typeof(TReader).GetMethod(name);
            if (method == null)
            {
                throw new InvalidOperationException(String.Format(CSI.Properties.Resources.CanNotFoundMethodInType,  name, typeof(TReader).FullName));
            }
            return (T) method.Invoke(reader, new object[] { column });
        }

        public static byte GetByte<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetByte<TReader>(reader, column, 0);
        }

        public static byte GetByte<TReader>(TReader reader, int column, byte nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return reader.GetByte(column);
            }
            return nullValue;
        }

        public static DateTime GetDateTime<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetDateTime<TReader>(reader, column, DateTime.MinValue);
        }

        public static DateTime GetDateTime<TReader>(TReader reader, int column, DateTime nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return reader.GetDateTime(column);
            }
            return nullValue;
        }

        public static int GetInt32<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetInt32<TReader>(reader, column, 0);
        }

        public static int GetInt32<TReader>(TReader reader, int column, int nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return reader.GetInt32(column);
            }
            return nullValue;
        }

        public static byte? GetNullableByte<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetNullableByte<TReader>(reader, column, 0);
        }

        public static byte? GetNullableByte<TReader>(TReader reader, int column, byte? nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return new byte?(reader.GetByte(column));
            }
            if (nullValue.HasValue)
            {
                return new byte?(nullValue.Value);
            }
            return null;
        }

        public static DateTime? GetNullableDateTime<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetNullableDateTime<TReader>(reader, column, null);
        }

        public static DateTime? GetNullableDateTime<TReader>(TReader reader, int column, DateTime? nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return new DateTime?(reader.GetDateTime(column));
            }
            if (nullValue.HasValue)
            {
                return new DateTime?(nullValue.Value);
            }
            return null;
        }

        public static int? GetNullableInt32<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetNullableInt32<TReader>(reader, column, null);
        }

        public static int? GetNullableInt32<TReader>(TReader reader, int column, int? nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return new int?(reader.GetInt32(column));
            }
            if (nullValue.HasValue)
            {
                return new int?(nullValue.Value);
            }
            return null;
        }

        public static string GetString<TReader>(TReader reader, int column) where TReader: DbDataReader
        {
            return GetString<TReader>(reader, column, string.Empty);
        }

        public static string GetString<TReader>(TReader reader, int column, string nullValue) where TReader: DbDataReader
        {
            if (!reader.IsDBNull(column))
            {
                return reader.GetString(column);
            }
            return nullValue;
        }
    }
}

