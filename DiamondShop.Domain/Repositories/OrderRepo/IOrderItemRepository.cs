using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        public Task<bool> Existing(JewelryId jewelryId);
        public Task CreateRange(List<OrderItem> orderItems);
        public void UpdateRange(List<OrderItem> orderItems);
        Task<List<OrderItem>> GetOrderItemsDetail(Order order, CancellationToken cancellationToken = default);
    }
}
