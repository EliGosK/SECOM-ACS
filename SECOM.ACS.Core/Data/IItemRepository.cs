using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IItemRepository : IRepository<Item,int>
    {
        IEnumerable<ItemDataView> GetByCriteria(ItemSearchCriteria criteria);
        bool IsInUse(int ItemID);
        bool IsDuplicate(Item entity);
    }
}
