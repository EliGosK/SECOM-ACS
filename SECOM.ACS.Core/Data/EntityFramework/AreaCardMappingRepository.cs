using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AreaCardMappingRepository : EntityRepository<ACSContext,AreaCardMapping>, IAreaCardMappingRepository
    {
        public AreaCardMappingRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AreaCardMapping mapping)
        {
            Context.InsertAreaCardMapping(mapping.CardID,mapping.AreaID,mapping.IsMainArea);
        }

        public void Deletes(string cardId)
        {
            Context.DeleteAreaCardMapping(cardId);
        }
    }
}
