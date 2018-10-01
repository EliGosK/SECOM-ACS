using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data.EntityFramework
{
    public class SystemMiscRepository : EntityRepository<ACSContext,SystemMisc>, ISystemMiscRepository
    {
        public SystemMiscRepository(ACSContext context) : base(context)
        {

        }

        public SystemMisc Get(SystemMiscKey key)
        {
            return Context.SystemMiscs.Where(t => String.Compare(key.SysMiscType, key.SysMiscType, true) == 0 && String.Compare(t.SysMiscCode, key.SysMiscCode, true) == 0).FirstOrDefault();
        }

        public IEnumerable<SystemMisc> GetByMiscType(string miscType)
        {
            return Context.SystemMiscs.Where(t => String.Compare(t.SysMiscType, miscType, true) == 0).ToList();

        }

        public IEnumerable<SystemMisc> GetCardTypeForCreate()
        {
            return Context.GetCardTypeForCreate().ToList();
        }

        public IEnumerable<SystemMisc> GetCardTypeForSearch()
        {
            return Context.GetCardTypeForSearch().ToList();
        }

        public IEnumerable<EntryTimeSetting> GetDefaultEntryTime()
        {
            return Context.GetDefaultEntryTime().ToList();
        }
    }
}
