using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondRepository : IBaseRepository<Diamond>
    {
        public void UpdateRange(List<Diamond> diamonds);
    }
}
