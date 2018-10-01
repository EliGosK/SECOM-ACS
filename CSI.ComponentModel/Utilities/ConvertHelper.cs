using CSI.Properties;
using CSI.Resources;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace CSI.Utilities
{
    public class ConvertHelper
    {
        public static TC ChangeType<T, TC>(T v)
        {
            return (TC) Convert.ChangeType(v, typeof(TC));
        }

        public static DateTime ConvertIntToDateTime(int date)
        {
            string pattern = @"^(?<Year>\d{4})(?<Month>\d{2})(?<Day>\d{2})$";
            string input = string.Format("{0:00000000}", date);
            Match match = Regex.Match(input, pattern);
            if (!match.Success)
            {
                throw new FormatException(StringResourceHelper.GetString(typeof(CSI.Properties.Resources), "InvalidDateIntValueMessage", new object[] { input }));
            }
            int year = int.Parse(match.Groups["Year"].Value);
            int month = int.Parse(match.Groups["Month"].Value);
            int day = int.Parse(match.Groups["Day"].Value);
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            DateTime time = new DateTime(year, month, day);
            Thread.CurrentThread.CurrentCulture = currentCulture;
            return time;
        }

        public static string DateTimeToTimeString(DateTime time)
        {
            return string.Format("{0:00}:{1:00}", time.Hour, time.Minute);
        }

        public static string TimeSpanToTimeString(TimeSpan time)
        {
            return string.Format("{0:00}:{1:00}", time.Hours, time.Minutes);
        }

        public static DateTime TimeStringToDateTime(string time)
        {
            string pattern = @"(?<Hour>\d{1,2}):(?<Minute>\d{2})";
            Match match = Regex.Match(time, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new Exception("Can not covert TimeStamp to DateTIme");
            }
            return new DateTime(1, 1, 1, int.Parse(match.Groups["Hour"].Value), int.Parse(match.Groups["Minute"].Value), 0);
        }

        public static TimeSpan TimeStringToTimeSpan(string time)
        {
            string pattern = @"(?<Hour>\d{1,2}):(?<Minute>\d{2})";
            Match match = Regex.Match(time, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                throw new Exception("Can not covert String to TimeSpan");
            }
            return new TimeSpan(int.Parse(match.Groups["Hour"].Value), int.Parse(match.Groups["Minute"].Value), 0);
        }

        public static string ToBinary(int value)
        {
            string str = string.Empty;
            while (value > 0)
            {
                long num = value % 2;
                str = str + num;
                value /= 2;
            }
            char[] array = str.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        public static int ToHexadecimal(string hexadecimal)
        {
            return Convert.ToInt32(hexadecimal, 0x10);
        }
    }
}

