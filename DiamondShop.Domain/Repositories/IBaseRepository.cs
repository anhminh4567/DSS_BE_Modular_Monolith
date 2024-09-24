using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll(CancellationToken token = default);

        Task<TEntity> GetById( params object[] ids);
        IQueryable<TEntity> GetQuery();
        IQueryable<TEntity> QueryFilter(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> filter = null);
        IQueryable<TEntity> QueryOrderBy(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Task Create(TEntity entity, CancellationToken token = default);
        Task Update(TEntity entity, CancellationToken token = default);
        Task Delete(TEntity entity, CancellationToken token = default);
        int GetCount();
    }
}
