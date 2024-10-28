using DiamondShop.Domain.Models.DiamondPrices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondCriteriaRepository : IBaseRepository<DiamondCriteria>
    {
        Task CreateMany(List<DiamondCriteria> diamondCriterias);
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCriteria(CancellationToken cancellationToken = default);
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableSideDiamondCriteria(CancellationToken cancellationToken = default);

    }
}
