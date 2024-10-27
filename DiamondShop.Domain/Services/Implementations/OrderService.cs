using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Repositories.JewelryRepo;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Services.interfaces;
using FluentResults;

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
        public async Task CancelItems(Order order, IOrderRepository _orderRepo, IOrderItemRepository _itemRepo, IJewelryRepository _jewelRepo, IDiamondRepository _diamondRepo)
        {
            var orderItemQuery = _itemRepo.GetQuery();
            orderItemQuery = _itemRepo.QueryFilter(orderItemQuery, p => p.OrderId == order.Id);
            var items = orderItemQuery.ToList();
            List<IError> errors = new();
            List<Jewelry> jewelries = new List<Jewelry>();
            List<Diamond> diamonds = new List<Diamond>();
            foreach (var item in items)
            {
                if (item.Status == OrderItemStatus.Pending || item.Status == OrderItemStatus.Prepared)
                {
                    if (item.JewelryId != null)
                    {
                        var jewelry = await _jewelRepo.GetById(item.JewelryId);
                        if (jewelry == null)
                            errors.Append(new Error($"Can't find jewelry #{item.JewelryId}"));
                        else
                        {
                            if (jewelry.Diamonds != null)
                            {
                                foreach (var diamond in jewelry.Diamonds)
                                {
                                    diamond.SetSell();
                                    diamonds.Add(diamond);
                                }
                            }
                            jewelry.SetSell();
                            jewelries.Add(jewelry);
                        }
                    }
                    if (item.DiamondId != null)
                    {
                        var diamond = await _diamondRepo.GetById(item.DiamondId);
                        if (diamond == null)
                            errors.Append(new Error($"Can't find diamond #{item.JewelryId}"));
                        else
                        {
                            diamond.SetSell();
                            diamonds.Add(diamond);
                        }
                    }
                    item.Status = OrderItemStatus.Removed;
                }
            }
            _itemRepo.UpdateRange(items);
            _jewelRepo.UpdateRange(jewelries);
            _diamondRepo.UpdateRange(diamonds);
        }
    }
}
