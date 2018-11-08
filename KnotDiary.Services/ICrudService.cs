using KnotDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KnotDiary.Services
{
    public interface ICrudService<TEntity> where TEntity : IBaseEntity
    {
        Task<IEnumerable<TEntity>> GetAll(int skip = 0, int take = 15);
        Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter = null, int skip = 0, int take = 15);
        Task<IEnumerable<TEntity>> GetAll(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, object>> orderBy = null, int skip = 0, int take = 15);
        Task<TEntity> Get(string id);
        Task<TEntity> Create(TEntity model);
        Task<TEntity> Update(TEntity model);
        Task<bool> Delete(string id);
    }
}
