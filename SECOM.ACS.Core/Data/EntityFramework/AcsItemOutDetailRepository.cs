using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsItemOutDetailRepository : EntityRepository<ACSContext, AcsItemOutDetail, string>, IAcsItemOutDetailRepository
    {

        public AcsItemOutDetailRepository(ACSContext context) : base(context)
        {

        }

        public void InsertAcsItemOutDetail(AcsItemOutDetail entity)
        {
            Context.InsertAcsItemOutDetail(entity.ReqNo, entity.Seq, entity.ItemID, entity.Description, entity.ConfdtFlag, entity.ReqQty, entity.PlanReturnDate, entity.ActOutQty, entity.ActReturnQty, entity.ActReturnDate, entity.UpdateBy);
        }

        public void UpdateAcsItemOutDetail(AcsItemOutDetail entity)
        {
            Context.UpdateAcsItemOutDetail(entity.DetailID, entity.Seq, entity.ItemID, entity.Description, entity.ConfdtFlag, entity.ReqQty, entity.PlanReturnDate, entity.ActOutQty, entity.ActReturnQty, entity.ActReturnDate, entity.UpdateBy);
        }

        public void DeleteAcsItemOutDetail(string RequestNo)
        {
            Context.DeleteAcsItemOutDetail(RequestNo);
        }

        public IEnumerable<AcsItemOutDetailDataView> GetDataViews(string requestNo)
        {
            return Context.GetAcsItemOutDetailDataViews(requestNo);
        }

    }
}
