using System;
using System.Text;

namespace CSI.Exceptions
{
    public static class ExceptionUtility
    {
        public static void FailFast(string message)
        {
            Environment.FailFast(message);
        }

        public static string FormatMessage(Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            return string.Format("{0}: {1}", e.GetType().Name, e.Message);
        }

        public static string GetExceptionMessage(Exception ex)
        {
            return GetExceptionMessage(ex, 0);
        }

        public static string GetExceptionMessage(Exception ex, int startingLevel)
        {
            int num = 0;
            StringBuilder sb = new StringBuilder();
            if (num >= startingLevel)
            {
                sb.Append(ex.Message);
                sb.AppendLine();
            }
            if (ex.InnerException != null)
            {
                GetExceptionMessage(ex.InnerException, sb, num + 1, startingLevel);
            }
            return sb.ToString();
        }

        private static void GetExceptionMessage(Exception ex, StringBuilder sb, int currentLevel, int startingLevel)
        {
            if (currentLevel >= startingLevel)
            {
                sb.Append(ex.Message);
                sb.AppendLine();
            }
            if (ex.InnerException != null)
            {
                GetExceptionMessage(ex.InnerException, sb, currentLevel + 1, startingLevel);
            }
        }

        public static Exception GetLastException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }

        public static string GetLastExceptionMessage(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex.Message;
        }

        public static string GetLastExceptionMessage(Exception ex, int iMaxLength)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            string message = ex.Message;
            if (message.Length < iMaxLength)
            {
                return message;
            }
            return message.Substring(0, iMaxLength);
        }

        public static ArgumentException ThrowHelperArgument(string message)
        {
            return (ArgumentException) ThrowHelperError(new ArgumentException(message));
        }

        public static ArgumentException ThrowHelperArgument(string message, Exception innerException)
        {
            return (ArgumentException) ThrowHelperError(new ArgumentException(message, innerException));
        }

        public static ArgumentException ThrowHelperArgument(string paramName, string message)
        {
            return (ArgumentException) ThrowHelperError(new ArgumentException(message, paramName));
        }

        public static Exception ThrowHelperError(Exception exception)
        {
            return exception;
        }
    }
}

