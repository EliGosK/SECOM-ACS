using CsvHelper.Configuration;
using SECOM.ACS.Tasks.TypeConversions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.ClassMaps
{
    public class EmployeeImportDataMap : CsvClassMap<EmployeeImportData>
    {
        public string[] DateFormats { get; set; } = new string[] { "yyyy-MM-dd", "dd/MM/yyyy" };
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        public DateTime DefaultDate { get; set; } = new DateTime(1900, 1, 1);

        public EmployeeImportDataMap()
        {
            Map(m => m.EmpID).Name("EmpID","Emp ID");
            Map(m => m.CardID).Name("CardID","Card ID");
            Map(m => m.Gender);
            Map(m => m.EmpNameTH).Name("EmpNameTH", "Emp Name TH","Employee Name TH");
            Map(m => m.EmpNameEN).Name("EmpNameEN", "Emp Name EN", "Employee Name EN");
            Map(m => m.Email);
            Map(m => m.Department);
            Map(m => m.Position);
            Map(m => m.SpecialPosition).Name("SpecialPosition", "Special Position");
            Map(m => m.Level);
            Map(m => m.StartWorkingDate).Name("StartWorkingDate", "Start Working Date").TypeConverter(new DateTimeConverter(this.DateFormats,this.Culture,this.DefaultDate));
            Map(m => m.ResignDate).Name("ResignDate","Resign Date").TypeConverter(new NullableDateTimeConverter(this.DateFormats,this.Culture));

            Map(m => m.Area1).Name("Area1", "Area1(Main)").TypeConverter<AreaConverter>();
            Map(m => m.Area2).TypeConverter<AreaConverter>();
            Map(m => m.Area3).TypeConverter<AreaConverter>();
            Map(m => m.Area4).TypeConverter<AreaConverter>();
            Map(m => m.Area5).TypeConverter<AreaConverter>();
            Map(m => m.Area6).TypeConverter<AreaConverter>();
            Map(m => m.Area7).TypeConverter<AreaConverter>();
            Map(m => m.Area8).TypeConverter<AreaConverter>();
            Map(m => m.Area9).TypeConverter<AreaConverter>();
            Map(m => m.Area10).TypeConverter<AreaConverter>();

            Map(m => m.Area11).TypeConverter<AreaConverter>();
            Map(m => m.Area12).TypeConverter<AreaConverter>();
            Map(m => m.Area13).TypeConverter<AreaConverter>();
            Map(m => m.Area14).TypeConverter<AreaConverter>();
            Map(m => m.Area15).TypeConverter<AreaConverter>();
            Map(m => m.Area16).TypeConverter<AreaConverter>();
            Map(m => m.Area17).TypeConverter<AreaConverter>();
            Map(m => m.Area18).TypeConverter<AreaConverter>();
            Map(m => m.Area19).TypeConverter<AreaConverter>();
            Map(m => m.Area20).TypeConverter<AreaConverter>();

            Map(m => m.Area21).TypeConverter<AreaConverter>();
            Map(m => m.Area22).TypeConverter<AreaConverter>();
            Map(m => m.Area23).TypeConverter<AreaConverter>();
            Map(m => m.Area24).TypeConverter<AreaConverter>();
            Map(m => m.Area25).TypeConverter<AreaConverter>();
            Map(m => m.Area26).TypeConverter<AreaConverter>();
            Map(m => m.Area27).TypeConverter<AreaConverter>();
            Map(m => m.Area28).TypeConverter<AreaConverter>();
            Map(m => m.Area29).TypeConverter<AreaConverter>();
            Map(m => m.Area30).TypeConverter<AreaConverter>();

            Map(m => m.Area31).TypeConverter<AreaConverter>();
            Map(m => m.Area32).TypeConverter<AreaConverter>();
            Map(m => m.Area33).TypeConverter<AreaConverter>();
            Map(m => m.Area34).TypeConverter<AreaConverter>();
            Map(m => m.Area35).TypeConverter<AreaConverter>();
            Map(m => m.Area36).TypeConverter<AreaConverter>();
            Map(m => m.Area37).TypeConverter<AreaConverter>();
            Map(m => m.Area38).TypeConverter<AreaConverter>();
            Map(m => m.Area39).TypeConverter<AreaConverter>();
            Map(m => m.Area40).TypeConverter<AreaConverter>();

            Map(m => m.Area41).TypeConverter<AreaConverter>();
            Map(m => m.Area42).TypeConverter<AreaConverter>();
            Map(m => m.Area43).TypeConverter<AreaConverter>();
            Map(m => m.Area44).TypeConverter<AreaConverter>();
            Map(m => m.Area45).TypeConverter<AreaConverter>();
            Map(m => m.Area46).TypeConverter<AreaConverter>();
            Map(m => m.Area47).TypeConverter<AreaConverter>();
            Map(m => m.Area48).TypeConverter<AreaConverter>();
            Map(m => m.Area49).TypeConverter<AreaConverter>();
            Map(m => m.Area50).TypeConverter<AreaConverter>();
        }
    }
}
