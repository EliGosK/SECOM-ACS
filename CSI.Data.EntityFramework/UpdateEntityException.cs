using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data.EntityFramework
{
    public class UpdateEntityException<T> : Exception
        where T : class
    {
        public UpdateEntityException(T data, string message) : base(message)
        {
            this.EntityToUpdated = data;
        }

        public T EntityToUpdated { get; private set; }
    }
}
