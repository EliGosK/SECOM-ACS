using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data.EntityFramework
{
    

    public class InsertEntityException<T> : Exception
        where T : class
    {
        public InsertEntityException(T data, string message) : base(message)
        {
            this.EntityToInserted = data;
        }

        public T EntityToInserted { get; private set; }
    }

   
}
