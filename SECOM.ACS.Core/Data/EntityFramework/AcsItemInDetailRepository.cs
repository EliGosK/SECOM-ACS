using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsItemInDetailRepository : EntityRepository<ACSContext, AcsItemInDetail, string>, IAcsItemInDetailRepository
    {

        public AcsItemInDetailRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsItemInDetail entity)
        {
            Context.InsertAcsItemInDetail(entity.ReqNo, entity.Seq, entity.ItemID, entity.Description, entity.ConfdtFlag, entity.ReqQty, entity.PlanReturnDate, entity.ActInQty, entity.ActReturnQty, entity.ActReturnDate, entity.UpdateBy);
        }

        public IEnumerable<AcsItemInDetailDataView> GetDataViews(string requestNo)
        {
            return Context.GetAcsItemInDetailDataViews(requestNo).ToList();
        }

        public void RemovesByRequestNo(string requestNo)
        {
            Context.DeleteAcsItemInDetail(requestNo);
            // Call store procedure for remove AcsItemInDetail by requestno
        }
        public void UpdateAcsItemInDetail(AcsItemInDetail entity)
        {
            Context.UpdateAcsItemInDetail(entity.DetailID,entity.Seq, entity.ItemID, entity.Description, entity.ConfdtFlag, entity.ReqQty, entity.PlanReturnDate, entity.ActInQty, entity.ActReturnQty, entity.ActReturnDate, entity.UpdateBy);
        }
    }
}
