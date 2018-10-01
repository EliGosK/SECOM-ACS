using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Data.EntityFramework
{
    public abstract class EntityRepository<TContext,TEntity, TKey> : EntityRepository<TContext,TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        public EntityRepository(TContext context) : base(context)
        {

        }

        public virtual TEntity Get(TKey id)
        {
            return Context.Set<TEntity>().Find(id);
        }


    }
}
