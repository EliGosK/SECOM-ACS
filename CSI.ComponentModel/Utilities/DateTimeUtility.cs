using System;

namespace CSI.Utilities
{
    public static class DateTimeUtility
    {
        public static DateTime GetFirstDateOfMonth(int monthOffset = 0)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthOffset);
        }

        public static DateTime GetLastDateOfMonth(int monthOffset = 0)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(monthOffset + 1).AddDays(-1);
        }

        public static int[] Calculate(DateTime date)
        {
            return Calculate(date, DateTime.Today);
        }

        public static int[] Calculate(DateTime date1, DateTime date2)
        {
            if (DateTime.Compare(date1, date2) > 0)
            {
                return new int[3];
            }
            int num = date2.Year - date1.Year;
            int months = 0;
            int days = 0;
            if ((date2 < date1.AddYears(num)) && (num != 0))
            {
                num--;
            }
            date1 = date1.AddYears(num);
            if (date1.Year == date2.Year)
            {
                months = date2.Month - date1.Month;
            }
            else
            {
                months = (12 - date1.Month) + date2.Month;
            }
            if ((date2 < date1.AddMonths(months)) && (months != 0))
            {
                months--;
            }
            date1 = date1.AddMonths(months);
            TimeSpan span = (TimeSpan) (date2 - date1);
            days = span.Days;
            return new int[] { num, months, days };
        }

        public static int[] Calculate(int year, int month, int day)
        {
            return Calculate(new DateTime(year, month, day));
        }

        public static string GetString(DateTime date)
        {
            return GetString(CSI.Properties.Resources.CalculateAgeFormat, date);
        }

        public static string GetString(string format, DateTime date)
        {
            int[] numArray = Calculate(date);
            return string.Format(format, numArray[0], numArray[1], numArray[2]);
        }

        public static string GetAgeString(int year, int month, int day)
        {
            return GetString(CSI.Properties.Resources.CalculateAgeFormat, year, month, day);
        }

        public static string GetString(string format, int year, int month, int day)
        {
            int[] numArray = Calculate(year, month, day);
            return string.Format(format, numArray[0], numArray[1], numArray[2]);
        }
    }
}

