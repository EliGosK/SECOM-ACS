using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsPhotoRepository : EntityRepository<ACSContext,AcsPhoto, string>, IAcsPhotoRepository
    {
        public AcsPhotoRepository(ACSContext context) : base(context)
        {

        }

        public override void Add(AcsPhoto entity)
        {
            var result = Context.InsertAcsPhoto(entity.Status, entity.TakePhotoDateFrom, entity.TakePhotoTimeFrom, entity.TakePhotoDateTo, entity.TakePhotoTimeTo, entity.AreaID, entity.PhotoByType, entity.PhotoEmpID, entity.TakePhotoName, entity.WitnessEmpID, entity.TargetItem, entity.EquipItemID, entity.OtherEquip, entity.PurposeCodeID, entity.OtherPurpose, entity.IsLending, entity.Note, entity.CreateBy,entity.AckBy).FirstOrDefault();
            if (!String.IsNullOrEmpty(result))
            {
                entity.ReqNo = result;
            }
        }

        public void Update(AcsPhoto entity)
        {
            Context.UpdateAcsPhoto(entity.ReqNo,entity.Status,entity.TakePhotoDateFrom,entity.TakePhotoTimeFrom,entity.TakePhotoDateTo,entity.TakePhotoTimeTo,entity.IsLending,entity.AssetCode,entity.ActReturnDate,entity.PurposeCodeID,entity.OtherPurpose,entity.Note,entity.ForceDoneBy,entity.ForceDoneDate,entity.UpdateBy);
        }
        
    }
}
