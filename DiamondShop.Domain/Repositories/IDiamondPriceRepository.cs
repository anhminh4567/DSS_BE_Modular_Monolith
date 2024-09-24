using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondPriceRepository : IBaseRepository<DiamondPrice>
    {
        Task<List<DiamondPrice>> GetPriceByShapes(DiamondShape shape, CancellationToken token = default);
    }
}
