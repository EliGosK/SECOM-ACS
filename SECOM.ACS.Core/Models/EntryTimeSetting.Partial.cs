using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class EntryTimeSetting
    {
        public int EntryTimeFromTotalMinutes
        {
            get {
                return Convert.ToInt32(GetValue(this.EntryTimeFrom).TotalMinutes);
            }
        }

        public int EntryTimeToTotalMinutes
        {
            get
            {
                return Convert.ToInt32(GetValue(this.EntryTimeTo).TotalMinutes);
            }
        }

        public TimeSpan EntryTimeFromTimespan
        {
            get
            {
                return GetValue(this.EntryTimeFrom);
            }
        }

        public TimeSpan EntryTimeToTimespan
        {
            get
            {
                return GetValue(this.EntryTimeTo);
            }
        }

        private TimeSpan GetValue(string data)
        {
            var match = Regex.Match((string)data, @"^((?<hours>\d{1,2}):(?<minutes>\d{1,2}))$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new TimeSpan(int.Parse(match.Groups["hours"].Value), int.Parse(match.Groups["minutes"].Value), 0);
            }
            return TimeSpan.MinValue;
        }
    }
}
