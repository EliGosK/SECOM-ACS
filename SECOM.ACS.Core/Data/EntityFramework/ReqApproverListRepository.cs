using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class ReqApproverListRepository : EntityRepository<ACSContext, ReqApproverList>, IReqApproverListRepository
    {
        public ReqApproverListRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(ReqApproverList entity)
        {
            Context.InsertReqApproverList(entity.ReqNo, entity.Step, entity.ApproveUserName, entity.AreaID,entity.ReferenceApprovalID, entity.CreateBy);
            //base.Add(entity);
        }

        public void RemovesByRequestNo(string requestNo)
        {
            Context.DeleteReqApproverList(requestNo);
        }

        public IEnumerable<ReqApproverList> GetLatest(string requestNo)
        {
            return Context.GetLatestReqApproverList(requestNo);
        }
    }
}
