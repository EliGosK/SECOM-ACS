using CSI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CSI.Data
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void Remove(TEntity entity);
        void Edit(TEntity area);
    }

    public interface IRepository<TEntity,TKey> : IRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(TKey key);
    }

    public interface IRepository<TEntity, TKey , TSearchCriteria> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        IEnumerable<TEntity> GetBySearchCriteria(TSearchCriteria criteria);
    }

    public interface IPagedListRepository<TEntity, TKey, TSearchCriteria> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        IPagedList<TEntity> GetPagedListBySearchCriteria(TSearchCriteria criteria);
    }
}
