using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Configuration
{
    public class KendoUIConfiguration
    {
        //public string Culture { get; set; } = "en";

        //public string GridShortDateFormat { get; set; } = "{0:d/M/yyyy}";
        //public string GridShortDateTimeFormat { get; set; } = "{0:d/M/yyyy HH:mm}";

        public TimeSpan TimePickerMinTime { get; set; } = new TimeSpan(0,0,0);
        public TimeSpan TimePickerMaxTime { get; set; } = new TimeSpan(23,0,0);
        public TimeSpan TimePickerDefaultStartTime { get; set; } = TimeSpan.FromHours(8);
        public TimeSpan TimePickerDefaultEndTime { get; set; } = TimeSpan.FromHours(17);
        public int TimePickerInterval { get; set; } = 60;

        public int AutoCompleteMinLengthForEmployeeName { get; set; } = 3;
        public int DropDownListMinLengthForEmployeeName { get; set; } = 3;

       
    }
}
