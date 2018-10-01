using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.ComponentModel
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException() : base("Data to insert or update is duplicate")
        {

        }
    }
}
