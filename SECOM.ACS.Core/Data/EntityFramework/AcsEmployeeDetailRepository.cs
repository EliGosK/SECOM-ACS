using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsEmployeeDetailRepository : EntityRepository<ACSContext,AcsEmployeeDetail,string>, IAcsEmployeeDetailRepository
    {
        public AcsEmployeeDetailRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsEmployeeDetail entity)
        {
            Context.InsertAcsEmployeeDetail(entity.ReqNo, entity.Seq, entity.EmpID, entity.Name, entity.DeptName);
        }
        
        public void RemovesByRequestNo(string requestNo)
        {
            Context.DeleteAcsEmployeeDetail(requestNo);
        }

        public IEnumerable<AcsEmployeeDetailDataView> GetDataViews(string requestNo)
        {
            return Context.GetAcsEmployeeDetailDataViews(requestNo);
        }

    }
}
