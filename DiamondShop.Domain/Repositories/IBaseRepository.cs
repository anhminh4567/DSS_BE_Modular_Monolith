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
        Task Create(TEntity entity, CancellationToken token = default);
        Task Update(TEntity entity, CancellationToken token = default);
        Task Delete(TEntity entity, CancellationToken token = default);
        int GetCount();
    }
}
