using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsItemOutRepository : EntityRepository<ACSContext,AcsItemOut, string>, IAcsItemOutRepository
    {
        public AcsItemOutRepository(ACSContext context) : base(context)
        {

        }

        public void InsertAcsItemOut(AcsItemOut entity)
        {
            var result = Context.InsertAcsItemOut(RequestStatus.Requesting, entity.TakeOutDate, entity.AreaID, entity.Company, entity.DeptName, entity.TakeOutName, entity.PurposeCodeID, entity.OtherPurpose, entity.Note, entity.CreateBy).FirstOrDefault();
            if (!String.IsNullOrEmpty(result))
            {
                entity.ReqNo = result;
            }
        }

        public void UpdateAcsItemOut(AcsItemOut entity)
        {
            Context.UpdateAcsItemOut(entity.ReqNo, entity.Status, entity.TakeOutDate, entity.AreaID, entity.Company, entity.DeptName, entity.TakeOutName, entity.PurposeCodeID, entity.OtherPurpose, entity.Note, entity.UpdateBy);
        }

        public IEnumerable<AcsItemOutDetailDataView> GetDataViews(string requestNo)
        {
            return Context.GetAcsItemOutDetailDataViews(requestNo);
        }
    }
}
