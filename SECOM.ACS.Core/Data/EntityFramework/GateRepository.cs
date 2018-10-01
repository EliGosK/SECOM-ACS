using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SECOM.ACS.Models;
using CSI.Data.EntityFramework;

namespace SECOM.ACS.Data.EntityFramework
{
    public class GateRepository : EntityRepository<ACSContext, Gate,string>, IGateRepository
    {
        public GateRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<Gate> GetByFactoryCode(string factoryCode)
        {
            return Context.Gates.Where(t => t.FactoryCode == factoryCode).ToList();
        }
    }
}
