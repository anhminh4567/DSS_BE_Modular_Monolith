using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
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
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableCaratRange(DiamondShape diamondShape, CancellationToken cancellationToken = default);//, Cut? cut
        Task<List<(float CaratFrom, float CaratTo)>> GroupAllAvailableSideDiamondCaratRange(CancellationToken cancellationToken = default);
        Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableCriteria(DiamondShape diamondShape, CancellationToken cancellationToken = default);//, Cut? cut
        Task<Dictionary<(float CaratFrom, float CaratTo), List<DiamondCriteria>>> GroupAllAvailableSideDiamondCriteria(CancellationToken cancellationToken = default);
        Task<List<DiamondCriteria>> GetCriteriasByManyId(List<DiamondCriteriaId> diamondCriteriaIds, CancellationToken cancellationToken = default);
    }
}
