using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;
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
        public Task<bool> Existing(JewelryId jewelryId)
        {
            return _set.Where(p => p.Status != OrderItemStatus.Removed && p.JewelryId == jewelryId).AnyAsync();
        }
        public Task<List<OrderItem>> GetOrderItemsDetail(Order order, CancellationToken cancellationToken = default)
        {
            return _set.Where(o => o.OrderId == order.Id)
                .Include(x => x.Diamond)
                .Include(x => x.Jewelry)
                    .ThenInclude(x => x.SideDiamond)
                .Include(x => x.Jewelry)
                    .ThenInclude(x => x.Diamonds)
                .ToListAsync();
        }

        public void UpdateRange(List<OrderItem> orderItems) => _set.UpdateRange(orderItems);

        public IQueryable<OrderItem> GetSoldJewelry()
        {
            var query = _set.AsQueryable();
            query = query.Include(p => p.Jewelry).ThenInclude(p => p.Metal);
            query = query.Include(p => p.Jewelry).ThenInclude(p => p.Model);
            query = query.Where(p => p.Status == OrderItemStatus.Done && p.JewelryId != null);
            return query.AsSplitQuery();
        }
    }
}
