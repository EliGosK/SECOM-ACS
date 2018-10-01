using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;
using CSI.Data;
using System.Data.Entity.Core.Objects;


namespace SECOM.ACS.Data.EntityFramework
{
    public class ItemRepository : EntityRepository<ACSContext,Item,int>, IItemRepository
    {
        public ItemRepository(ACSContext context) : base(context)
        {

        }
        
        public IEnumerable<ItemDataView> GetByCriteria(ItemSearchCriteria criteria)
        {
            return Context.GetItemDataViewsByCriteria(criteria.ItemName, criteria.ItemType, criteria.StatusValue, criteria.IsItemIn,criteria.IsItemOut,criteria.IsPhoto).ToList();
        }
        public override void Add(Item entity)
        {
   
            //base.Add(entity);
            Context.InsertItem(entity.ItemTypeID,entity.ItemName,entity.ItemDisplayEN,entity.ItemDisplayTH,entity.Description,entity.IsConfdt,entity.IsItemIn,entity.IsItemOut,entity.IsPhoto,entity.IsActive,entity.CreateBy,entity.CreateBy);
            
        }

        public override void Edit(Item entity)
        {
            Context.UpdateItem(entity.ItemID, entity.ItemTypeID, entity.ItemName, entity.ItemDisplayEN, entity.ItemDisplayTH, entity.Description, entity.IsConfdt, entity.IsItemIn, entity.IsItemOut, entity.IsPhoto, entity.IsActive, entity.CreateBy, entity.CreateBy);
        }

        public override void Remove(Item entity)
        {
            Context.DeleteItem(entity.ItemID);
        }

        public bool IsInUse(int ItemID)
        {
            var result = Context.CheckItemInUse(ItemID).FirstOrDefault();


            if (result.HasValue)
            {
                return result.Value;
            }
            return false;
      

        }

        public bool IsDuplicate(Item entity)
        {
            var result = Context.CheckDuplicateItem(entity.ItemTypeID,entity.ItemName).FirstOrDefault();
            if (result.HasValue)
            {
                return result.Value == 1;
            }
            return false;
        }
    }
}
