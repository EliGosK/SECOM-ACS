using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IMiscRepository : IRepository<Misc,int,MiscSearchCriteria>
    {
        bool IsInUse(int miscId);
        bool IsDuplicate(Misc entity);
    }
}
