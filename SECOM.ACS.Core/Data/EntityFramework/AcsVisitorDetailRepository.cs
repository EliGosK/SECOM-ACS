using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsVisitorDetailRepository : EntityRepository<ACSContext,AcsVisitorDetail,Guid>, IAcsVisitorDetailRepository
    {
        public AcsVisitorDetailRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsVisitorDetail entity)
        {
            Context.InsertAcsVisitorDetail(entity.ReqNo, entity.Seq, entity.VerifyTypeID, entity.VerifyNo,entity.Name,entity.Company ,entity.DeptName,entity.ItemInOut,entity.Photographing,entity.Description);
        }

        public void RemovesByRequestNo(string requestNo)
        {
            Context.DeleteAcsVisitorDetail(requestNo);
        }

        public IEnumerable<AcsVisitorDetailDataView> GetDataViews(string requestNo)
        {
            return Context.GetAcsVisitorDetailDataViews(requestNo);
        }
    }
}
