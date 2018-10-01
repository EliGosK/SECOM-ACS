using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited = true)]
    public class AcsTaskAttribute : Attribute
    {
        public bool CanRun { get; set; }
        public bool CanEdit { get; set; }
    }
}
