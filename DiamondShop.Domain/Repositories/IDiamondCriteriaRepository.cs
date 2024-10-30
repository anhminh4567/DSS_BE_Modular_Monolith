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
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCaratRange(CancellationToken cancellationToken = default);
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableSideDiamondCaratRange(CancellationToken cancellationToken = default);
        Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableCriteria(CancellationToken cancellationToken = default);
        Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableSideDiamondCriteria(CancellationToken cancellationToken = default);

    }
}
