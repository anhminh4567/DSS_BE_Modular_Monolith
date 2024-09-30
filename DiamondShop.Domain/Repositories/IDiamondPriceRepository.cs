using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondPriceRepository : IBaseRepository<DiamondPrice>
    {
        Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape, CancellationToken token = default);
        Task<DiamondPrice?> GetById(DiamondShapeId shapeId, DiamondCriteriaId criteriaId, CancellationToken cancellationToken =default);
        Task CreateMany(List<DiamondPrice> prices);
    }
}
