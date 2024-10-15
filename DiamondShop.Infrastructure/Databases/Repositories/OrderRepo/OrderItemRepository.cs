using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(DiamondShopDbContext dbContext) : base(dbContext) { }

        public async Task CreateRange(List<OrderItem> orderItems)
        {
            await _set.AddRangeAsync(orderItems);
        }
        public void UpdateRange(List<OrderItem> orderItems) => _set.UpdateRange(orderItems);

    }
}
