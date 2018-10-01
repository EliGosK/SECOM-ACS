using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsTransactionRepository : EntityRepository<ACSContext,TransactionAcs>, IAcsTransactionRepository
    {
        public AcsTransactionRepository(ACSContext context) : base(context)
        {

        }

        public TransactionAcs Get(Guid transactionId)
        {
            return Context.TransactionAcs.Where(t => t.TranID == transactionId).FirstOrDefault();
        }

        public IEnumerable<TransactionAcs> GetByRequestNo(string requestNo)
        {
            return Context.TransactionAcs.Where(t =>String.Compare( t.ReqNo ,requestNo,true)==0).ToList();
        }

       
    }
}
