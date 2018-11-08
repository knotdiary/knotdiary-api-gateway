using KnotDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KnotDiary.Data
{
    public interface IGenericRepository<TEntity> where TEntity : IBaseEntity
    {
        Task<IList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<TEntity, object>> orderBy = null,
            int skip = 0, int take = 10);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<TEntity> Insert(TEntity entity);

        Task<TEntity> Update(TEntity entity);

        Task<bool> Delete(object id);
    }
}
