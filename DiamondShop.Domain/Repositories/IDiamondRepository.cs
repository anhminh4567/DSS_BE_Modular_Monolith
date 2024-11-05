using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondRepository : IBaseRepository<Diamond>
    {
        Task<(Diamond diamond,List<Discount> discounts, List<Promotion> promotion)> GetByIdIncludeDiscountAndPromotion(DiamondId id, CancellationToken cancellationToken = default);
        public void UpdateRange(List<Diamond> diamonds);
        Task<List<Diamond>> GetDiamondsJewelry (JewelryId jewelryId, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetAllAdmin(CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetUserLockDiamonds(AccountId accountId, CancellationToken cancellationToken = default);
    }
}
