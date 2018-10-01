using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsItemInRepository : EntityRepository<ACSContext,AcsItemIn, string>, IAcsItemInRepository
    {
        public AcsItemInRepository(ACSContext context) : base(context)
        {

        }
        public override void Add(AcsItemIn model)
        {
            var result = Context.InsertAcsItemIn(model.Status, model.TakeInDate, model.AreaID, model.Company, model.DeptName, model.TakeInName, model.PurposeCodeID, model.OtherPurpose,model.Note, model.AckBy, model.CreateBy).FirstOrDefault();
            if (!String.IsNullOrEmpty(result))
            {
                model.ReqNo = result;
            }   
        }
    }
}
