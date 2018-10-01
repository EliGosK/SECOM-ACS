using CSI.Data.EntityFramework;
using CSI.Data.Extensions;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECOM.ACS.Data.EntityFramework
{
    public class PositionRepository : EntityRepository<ACSContext, Position, string>, IPositionRepository
    {
        public PositionRepository(ACSContext context) : base(context)
        {

        }
    }
}
