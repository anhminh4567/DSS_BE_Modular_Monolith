using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IOrderService
    {
        public bool IsCancellable(OrderStatus order);
        public bool IsProceedable(OrderStatus order);
        public bool CheckForSameCity(List<Order> orders);
        public Task CancelItems(Order order, IOrderRepository _orderRepo, IOrderItemRepository _itemRepo, IJewelryRepository _jewelRepo, IDiamondRepository _diamondRepo);
    }
}
