using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Web.Mvc.KendoUI
{
    public class FilterRequest
    {
        public List<FilterDescription> Filters { get; set; } = new List<FilterDescription>();
        public string Logic { get; set; }
    }

    public class FilterDescription
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool IgnoreCase { get; set; }
    }

    public class SortDescription
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }
}
