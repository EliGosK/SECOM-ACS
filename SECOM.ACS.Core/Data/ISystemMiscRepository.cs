using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface ISystemMiscRepository : IRepository<SystemMisc, SystemMiscKey>
    {
        IEnumerable<SystemMisc> GetByMiscType(string miscType);
        IEnumerable<SystemMisc> GetCardTypeForCreate();
        IEnumerable<SystemMisc> GetCardTypeForSearch();
        IEnumerable<EntryTimeSetting> GetDefaultEntryTime();
    }
}
