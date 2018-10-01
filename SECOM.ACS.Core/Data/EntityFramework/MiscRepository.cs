using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;
using System.Data.Entity.Core.Objects;

namespace SECOM.ACS.Data.EntityFramework
{
    public class MiscRepository : EntityRepository<ACSContext,Misc,int>, IMiscRepository
    {
        public MiscRepository(ACSContext context) : base(context)
        {
            
        }

        public IEnumerable<Misc> GetBySearchCriteria(MiscSearchCriteria criteria)
        {
            return Context.GetMiscsByCriteria(criteria.Type,criteria.ActiveStatus, MergeOption.NoTracking).ToList();
        }

        public override void Add(Misc entity)
        {
            Context.InsertMisc(entity.MiscType, entity.MiscName, entity.MiscDisplayEN, entity.MiscDisplayTH, entity.MiscRemark, entity.IsActive, entity.DeleteAble, entity.CreateBy);
        }

        public override void Edit(Misc entity)
        {
            Context.UpdateMisc(entity.MiscID, entity.MiscType, entity.MiscName, entity.MiscDisplayEN, entity.MiscDisplayTH, entity.MiscRemark, entity.IsActive, entity.DeleteAble, entity.UpdateBy);
        }

        public override void Remove(Misc entity)
        {
            Context.DeleteMisc(entity.MiscID);
        }

        public bool IsInUse(int miscId)
        {
            var result = Context.CheckMiscInUse(miscId).FirstOrDefault();
            if (result.HasValue)
            {
                return result.Value == 1;
            }
            return false;
        }

        public bool IsDuplicate(Misc entity)
        {
            var result = Context.CheckDuplicateMisc(entity.MiscType,entity.MiscName).FirstOrDefault();
            if (result.HasValue)
            {
                return result.Value == 1;
            }
            return false;
        }
    }
}
