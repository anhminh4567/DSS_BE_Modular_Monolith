using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Services.interfaces;

namespace DiamondShop.Domain.Services.Implementations
{
    public class OrderService : IOrderService
    {
        List<OrderStatus> cancellableState = new() {
            OrderStatus.Pending,
            OrderStatus.Processing,
            OrderStatus.Prepared,
            OrderStatus.Delivery_Failed,
        };
        public OrderService() { }

        public bool CheckForSameCity(List<Order> orders)
        {
            //TODO: MAKE IT
            return true;
        }

        public bool IsCancellable(OrderStatus status)
        {
            return cancellableState.Contains(status);
        }

    }
}
