using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;

namespace DiamondShop.Domain.Repositories
{
    public interface IDiamondRepository : IBaseRepository<Diamond>
    {
        Task<(Diamond diamond,List<Discount> discounts, List<Promotion> promotion)> GetByIdIncludeDiscountAndPromotion(DiamondId id, CancellationToken cancellationToken = default);
        public void UpdateRange(List<Diamond> diamonds);
        Task<int> GetCountByStatus(List<ProductStatus> diamondStatusesToLookFor, bool? isLab, bool includeAttachingToJewelry = true);
        Task<int> GetCountByShapeAndStatus(List<ProductStatus> diamondStatusesToLookFor, bool? isLab, List<DiamondShapeId> shapesToLookFor, bool includeAttachingToJewelry = true);
        IQueryable<Diamond> QueryStatus(IQueryable<Diamond> query ,List<ProductStatus> diamondStatusesToLookFor);
        Task<List<Diamond>> GetDiamondsJewelry (JewelryId jewelryId, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetAllAdmin(CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetUserLockDiamonds(AccountId accountId, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetLockDiamonds(CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetBySkus(string[] skus, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetRange(List<DiamondId> diamondIds, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetTotalSoldDiamonds(bool? isLab, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetTotalSoldDiamondsByShape(DiamondShape shape, bool? isLab,DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
        Task<List<Diamond>> GetWhereSkuContain(string containingString, int skip, int take, CancellationToken cancellationToken = default);
    }
}
