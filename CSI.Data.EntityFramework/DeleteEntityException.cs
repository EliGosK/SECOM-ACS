using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data.EntityFramework
{
    public class DeleteEntityException<T> : Exception
        where T : class
    {
        public DeleteEntityException(T data, string message) : base(message)
        {
            this.EntityToDeleted = data;
        }

        public T EntityToDeleted { get; private set; }
    }
}
