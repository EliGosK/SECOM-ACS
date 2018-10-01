using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsVisitorRepository : EntityRepository<ACSContext,AcsVisitor,string>, IAcsVisitorRepository
    {
        public AcsVisitorRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsVisitor entity)
        {
            var result = Context.InsertAcsVisitor(entity.Status,entity.EntryDateFrom, entity.EntryTimeFrom, entity.EntryDateTo, entity.EntryTimeTo, entity.PurposeCodeID, entity.OtherPurpose, entity.Note, entity.CreateBy).FirstOrDefault();
            if (!String.IsNullOrEmpty(result))
            {
                entity.ReqNo = result;
            }
        }

        public void Update(AcsVisitor entity)
        {
            Context.UpdateAcsVisitor(entity.ReqNo, entity.Status, entity.EntryDateFrom, entity.EntryTimeFrom, entity.EntryDateTo, entity.EntryTimeTo, entity.PurposeCodeID, entity.OtherPurpose, entity.Note, entity.UpdateBy);
        }

        public IEnumerable<AcsVisitorTransactionDataView> GetTransactionViews(string requestNo)
        {
            return Context.GetAcsVisitorTransactionDataViews(requestNo);
        }
    }
}
