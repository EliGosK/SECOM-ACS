using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsEmployeeRepository : EntityRepository<ACSContext,AcsEmployee,string>, IAcsEmployeeRepository
    {
        public AcsEmployeeRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsEmployee entity)
        {
            var result = Context.InsertAcsEmployee(entity.Status,entity.RequestFor,entity.EntryDateFrom,entity.EntryTimeFrom,entity.EntryDateTo,entity.EntryTimeTo,entity.PurposeCodeID,entity.OtherPurpose,entity.Note,entity.CreateBy).FirstOrDefault();
            if (!String.IsNullOrEmpty(result))
            {
                entity.ReqNo = result;
            }
        }
        
        public void Update(AcsEmployee entity)
        {
            Context.UpdateAcsEmployee(entity.ReqNo, entity.Status, entity.EntryDateFrom, entity.EntryTimeFrom, entity.EntryDateTo, entity.EntryTimeTo, entity.PurposeCodeID, entity.OtherPurpose, entity.Note, entity.UpdateBy);
        }
    }
}
