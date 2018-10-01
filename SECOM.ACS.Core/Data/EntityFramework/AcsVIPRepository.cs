using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsVIPRepository : EntityRepository<ACSContext,AcsVIP,string>, IAcsVIPRepository
    {
        public AcsVIPRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsVIP entity)
        {
            var result = Context.InsertAcsVIP(entity.Status, entity.Name, entity.PositionMiscID, entity.Company, entity.Description, entity.CreateBy).FirstOrDefault();
            if (result != null) {
                entity.ReqNo = result;
            }
        }

        public override void Edit(AcsVIP entity)
        {
            Context.UpdateAcsVIP(entity.ReqNo, entity.Status, entity.UpdateBy);
        }

        public IEnumerable<VIPCardRegistrationView> GetVIPCardRegistrationViews(VIPCardRegistrationSearchCriteria criteria)
        {
            var result = Context.GetVIPCardRegistrationView(criteria.EntryDate, CardType.VIP, criteria.CardStatus).ToList();
            return result;
        }

        public void InsertVIPTransactionAcs(AcsVIP entity)
        {
            Context.InsertVIPTransactionAcs(entity.ReqNo, entity.CardID, (int)TransactionStatus.SendCardToACS, entity.CreateBy);
        }

        public void UpdateVIPTransactionAcs(AcsVIP entity)
        {
            Context.UpdateVIPTransactionAcs(entity.ReqNo, entity.CardID, entity.UpdateBy);
        }
    }
}
