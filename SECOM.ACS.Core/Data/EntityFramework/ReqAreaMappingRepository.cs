using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class ReqAreaMappingRepository : EntityRepository<ACSContext, ReqAreaMapping>, IReqAreaMappingRepository
    {
        public ReqAreaMappingRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(ReqAreaMapping entity)
        {
            Context.InsertReqAreaMapping(entity.ReqNo,  entity.AreaID);
        }

        public void RemovesByRequestNo(string requestNo)
        {
            Context.DeleteReqAreaMappings(requestNo);
        }
    }
}
