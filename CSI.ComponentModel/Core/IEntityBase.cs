using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.Core
{
    public interface IEntityBase<T>
    {
       T Id { get; set; }
    }
}
