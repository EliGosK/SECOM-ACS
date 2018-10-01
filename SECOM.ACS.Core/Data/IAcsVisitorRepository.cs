using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAcsVisitorRepository : IRepository<AcsVisitor,string>
    {
        /// <summary>
        /// Update Acs Visitor
        /// </summary>
        /// <param name="entity"></param>
        void Update(AcsVisitor entity);

        IEnumerable<AcsVisitorTransactionDataView> GetTransactionViews(string requestNo);
    }
}
