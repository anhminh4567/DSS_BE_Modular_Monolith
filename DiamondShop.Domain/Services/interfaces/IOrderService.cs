using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;

namespace DiamondShop.Domain.Services.interfaces
{
    public interface IOrderService
    {
        public bool IsCancellable(OrderStatus order);
        public bool CheckForSameCity(List<Order> orders);
    }
}
