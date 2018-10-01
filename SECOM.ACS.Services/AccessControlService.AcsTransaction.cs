using CSI.ComponentModel;
using SECOM.ACS.Data;
using SECOM.ACS.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public partial class AccessControlService 
    {
        public TransactionAcs GetAcsTransaction(Guid transactionId)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsTransactions.Find(t => t.TranID == transactionId).FirstOrDefault();
            }
        }

        public IEnumerable<TransactionAcs> GetAcsTransactionsByRequestNo(string requestNo)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsTransactions.Find(t => String.Compare(t.ReqNo,requestNo,true)==0).ToList();
            }
        }

        public ObjectResult CreateAcsTransactions(params TransactionAcs[] entities)
        {
            var results = new ObjectResults<TransactionAcs>();
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    foreach (var entity in entities)
                    {
                        u.AcsTransactions.Add(entity);
                    }
                    u.Complete();
                }
                return ObjectResult.Succeed();
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
            
        }

        public ObjectResult UpdateAcsTransactions(params TransactionAcs[] entities)
        {
            var results = new ObjectResults<TransactionAcs>();
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    foreach (var entity in entities)
                    {
                        var transaction = u.AcsTransactions.Find(t => t.TranID == entity.TranID).FirstOrDefault();
                        if (transaction == null) { continue; }
                        // Modified acs transaction
                        transaction.Status = entity.Status;
                        transaction.SendAcsDate = entity.SendAcsDate;
                        transaction.CancelAcsDate = entity.CancelAcsDate;
                        transaction.UpdateBy = entity.UpdateBy;
                        transaction.UpdateDate = DateTime.Now;
                    }
                    u.Complete();
                }
                return ObjectResult.Succeed();
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }

        
    }
}
