using CsvHelper.Configuration;
using SECOM.ACS.Models;
using SECOM.ACS.Tasks.TypeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks.ClassMaps
{
    public class EmployeeForImportAcsMap : CsvClassMap<EmployeeForImportAcs>
    {
        public EmployeeForImportAcsMap() : this("DUMMY", "dd/MM/yyyy")
        {

        }

        public EmployeeForImportAcsMap(string dummyAccessGroup, string dateFormat)
        {
            var defaultStart = DateTime.Today.AddDays(-1);
            var defaultEnd = new DateTime(DateTime.Now.Year, 1, 1).AddDays(-1).AddYears(50);
            var accessGroupTypeConverter = new AcsGroupConverter(dummyAccessGroup);
            var startTypeGroupConverter = new AcsDateTimeConverter(defaultStart, dateFormat);
            var expireTypeGroupConverter = new AcsDateTimeConverter(defaultEnd, dateFormat);

            Map(m => m.EmpID).Name("EMP ID");
            Map(m => m.CardNo).Name("Card No.");
            Map(m => m.CardFormat).Name("Card Format");
            Map(m => m.FirstName).Name("First name");
            Map(m => m.LastName).Name("Last name");
            Map(m => m.Company);
            Map(m => m.Department);
            Map(m => m.Position);

            Map(m => m.AccessGroup1).Name("Access Group1").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup1).Name("Start Group1").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup1).Name("Expire Group1").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup2).Name("Access Group2").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup2).Name("Start Group2").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup2).Name("Expire Group2").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup3).Name("Access Group3").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup3).Name("Start Group3").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup3).Name("Expire Group3").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup4).Name("Access Group4").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup4).Name("Start Group4").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup4).Name("Expire Group4").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup5).Name("Access Group5").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup5).Name("Start Group5").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup5).Name("Expire Group5").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup6).Name("Access Group6").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup6).Name("Start Group6").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup6).Name("Expire Group6").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup7).Name("Access Group7").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup7).Name("Start Group7").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup7).Name("Expire Group7").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup8).Name("Access Group8").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup8).Name("Start Group8").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup8).Name("Expire Group8").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup9).Name("Access Group9").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup9).Name("Start Group9").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup9).Name("Expire Group9").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup10).Name("Access Group10").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup10).Name("Start Group10").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup10).Name("Expire Group10").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup11).Name("Access Group11").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup11).Name("Start Group11").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup11).Name("Expire Group11").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup12).Name("Access Group12").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup12).Name("Start Group12").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup12).Name("Expire Group12").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup13).Name("Access Group13").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup13).Name("Start Group13").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup13).Name("Expire Group13").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup14).Name("Access Group14").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup14).Name("Start Group14").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup14).Name("Expire Group14").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup15).Name("Access Group15").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup15).Name("Start Group15").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup15).Name("Expire Group15").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup16).Name("Access Group16").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup16).Name("Start Group16").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup16).Name("Expire Group16").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup17).Name("Access Group17").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup17).Name("Start Group17").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup17).Name("Expire Group17").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup18).Name("Access Group18").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup18).Name("Start Group18").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup18).Name("Expire Group18").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup19).Name("Access Group19").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup19).Name("Start Group19").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup19).Name("Expire Group19").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup20).Name("Access Group20").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup20).Name("Start Group20").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup20).Name("Expire Group20").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup21).Name("Access Group21").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup21).Name("Start Group21").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup21).Name("Expire Group21").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup22).Name("Access Group22").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup22).Name("Start Group22").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup22).Name("Expire Group22").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup23).Name("Access Group23").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup23).Name("Start Group23").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup23).Name("Expire Group23").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup24).Name("Access Group24").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup24).Name("Start Group24").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup24).Name("Expire Group24").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup25).Name("Access Group25").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup25).Name("Start Group25").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup25).Name("Expire Group25").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup26).Name("Access Group26").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup26).Name("Start Group26").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup26).Name("Expire Group26").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup27).Name("Access Group27").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup27).Name("Start Group27").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup27).Name("Expire Group27").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup28).Name("Access Group28").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup28).Name("Start Group28").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup28).Name("Expire Group28").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup29).Name("Access Group29").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup29).Name("Start Group29").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup29).Name("Expire Group29").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup30).Name("Access Group30").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup30).Name("Start Group30").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup30).Name("Expire Group30").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup31).Name("Access Group31").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup31).Name("Start Group31").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup31).Name("Expire Group31").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup32).Name("Access Group32").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup32).Name("Start Group32").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup32).Name("Expire Group32").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup33).Name("Access Group33").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup33).Name("Start Group33").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup33).Name("Expire Group33").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup34).Name("Access Group34").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup34).Name("Start Group34").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup34).Name("Expire Group34").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup35).Name("Access Group35").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup35).Name("Start Group35").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup35).Name("Expire Group35").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup36).Name("Access Group36").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup36).Name("Start Group36").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup36).Name("Expire Group36").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup37).Name("Access Group37").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup37).Name("Start Group37").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup37).Name("Expire Group37").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup38).Name("Access Group38").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup38).Name("Start Group38").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup38).Name("Expire Group38").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup39).Name("Access Group39").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup39).Name("Start Group39").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup39).Name("Expire Group39").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup40).Name("Access Group40").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup40).Name("Start Group40").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup40).Name("Expire Group40").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup41).Name("Access Group41").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup41).Name("Start Group41").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup41).Name("Expire Group41").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup42).Name("Access Group42").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup42).Name("Start Group42").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup42).Name("Expire Group42").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup43).Name("Access Group43").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup43).Name("Start Group43").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup43).Name("Expire Group43").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup44).Name("Access Group44").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup44).Name("Start Group44").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup44).Name("Expire Group44").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup45).Name("Access Group45").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup45).Name("Start Group45").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup45).Name("Expire Group45").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup46).Name("Access Group46").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup46).Name("Start Group46").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup46).Name("Expire Group46").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup47).Name("Access Group47").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup47).Name("Start Group47").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup47).Name("Expire Group47").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup48).Name("Access Group48").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup48).Name("Start Group48").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup48).Name("Expire Group48").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup49).Name("Access Group49").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup49).Name("Start Group49").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup49).Name("Expire Group49").TypeConverter(expireTypeGroupConverter);

            Map(m => m.AccessGroup50).Name("Access Group50").TypeConverter(accessGroupTypeConverter);
            Map(m => m.StartGroup50).Name("Start Group50").TypeConverter(startTypeGroupConverter);
            Map(m => m.ExpireGroup50).Name("Expire Group50").TypeConverter(expireTypeGroupConverter);

            Map(m => m.RootDivision).Name("Root Division");
        }
    }

}
