using DiamondShop.Domain.Models.Jewelries.Entities;

namespace DiamondShop.Domain.Repositories.JewelryRepo
{
    public interface IJewelrySideDiamondRepository : IBaseRepository<JewelrySideDiamond>
    {
        public Task CreateRange(List<JewelrySideDiamond> jewelrySideDiamonds);
    }
}
