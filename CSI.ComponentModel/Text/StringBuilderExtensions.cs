using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Text.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, string value)
        {
            if (condition)
            {
                return sb.AppendLine(value);
            }
            return sb;
        }

        public static StringBuilder AppendFormatIf(this StringBuilder sb, bool condition, string format, params object[] args)
        {
            if (condition)
            {
                return sb.AppendFormat(format, args);
            }
            return sb;
        }

        public static StringBuilder AppendWhereLineIf(this StringBuilder sb, bool condition ,string value)
        {
            if (condition) {
                if (sb.Length > 0) { sb.Append(" AND "); }
                return sb.AppendLine(value);
            }
            return sb;
        }

        public static StringBuilder AppendWhereFormatIf(this StringBuilder sb, bool condition, string format,params object[] args)
        {
            if (condition)
            {
                if (sb.Length > 0) { sb.Append(" AND "); }
                return sb.AppendFormat(format, args);
            }
            return sb;
        }

        public static string ToTitleCase(this string text)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(text);
        }
    }
}
