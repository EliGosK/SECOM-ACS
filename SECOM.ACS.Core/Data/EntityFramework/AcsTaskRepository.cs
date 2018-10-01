using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;
using System.Data.Entity.Core.Objects;
using CSI.Exceptions;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AcsTaskRepository : EntityRepository<ACSContext,AcsTask,string>, IAcsTaskRepository
    {
        public AcsTaskRepository(ACSContext context) : base(context)
        {
            
        }

        public bool UpdateDocument(int documentTypes,out DateTime updateDate)
        {
            updateDate = DateTime.MinValue;
            var result =  Context.UpdateDocument(documentTypes);
            var returnValue = result.FirstOrDefault();
            if (returnValue == null || !returnValue.HasValue)
            {
                return false;
            }
            updateDate = returnValue.Value;
            return true;
        }
    }
}
