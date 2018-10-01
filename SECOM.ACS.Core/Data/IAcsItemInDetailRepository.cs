using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAcsItemInDetailRepository : IRepository<AcsItemInDetail,string>
    {
        IEnumerable<AcsItemInDetailDataView> GetDataViews(string requestNo);
        void RemovesByRequestNo(string requestNo);
        void UpdateAcsItemInDetail(AcsItemInDetail entity);
    }
}
