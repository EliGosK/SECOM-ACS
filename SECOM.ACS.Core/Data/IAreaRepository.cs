using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAreaRepository : IRepository<Area,int, AreaSearchCriteria>
    {
        AreaDataView GetDataView(int areaId);
        IEnumerable<AreaDataView> GetDataViews(string user, bool areaMapping);
        IEnumerable<Area> Find(AreaSearchCriteria criteria);
        void LoadGate(Area area);
        void LoadAreaApprover(Area area);
        bool IsInUse(int AreaID);
        bool IsDuplicate(Area entity);
        IEnumerable<AreaMapping> GetAreaMapping(string id);
    }
}
