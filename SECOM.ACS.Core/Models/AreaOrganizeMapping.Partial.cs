using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class AreaOrganize
    {

    }

    public class AreaOrganizeSearchCriteria
    {
        public string DepartmentID { get; set; }
        public string PositionID { get; set; }
        public string SpecialPositionID { get; set; }
        public string Level { get; set; }
    }

}
