using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        IQueryable<Order> GetDetailQuery(IQueryable<Order> query, bool isIncludeJewelry = true, bool isIncludeDiamond = true);
        public Task<bool> IsOwner(AccountId accountId, JewelryId jewelryId);
        bool IsRequestCreated(CustomizeRequestId customizeRequestId);
        Task<Order?> GetDelivererCurrentlyHandledOrder(Account delivererAccount, CancellationToken cancellationToken = default);
        IQueryable<Order> GetSoldJewelry();
        Task<List<Order>> GetUserProcessingOrders(Account customerAccounts);
    }
}
