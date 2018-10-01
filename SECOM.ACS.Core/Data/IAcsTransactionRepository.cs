using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAcsTransactionRepository : IRepository<TransactionAcs,Guid>
    {
        IEnumerable<TransactionAcs> GetByRequestNo(string requestNo);
    }
}
