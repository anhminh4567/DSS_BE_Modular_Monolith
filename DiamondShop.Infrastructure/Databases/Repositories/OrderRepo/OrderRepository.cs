using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders;
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
    internal class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DiamondShopDbContext dbContext) : base(dbContext)
        {

        }

        public override Task<Order?> GetById(params object[] ids)
        {
            return _set.Include(o => o.Transactions).Include(x => x.Items).FirstOrDefaultAsync(o => o.Id == (OrderId)ids[0]);
        }
        public async Task<bool> IsOwner(AccountId accountId, JewelryId jewelryId)
        {
            var orders = await _set.Include(p => p.Items).Where(p => p.Status == OrderStatus.Success && p.AccountId == accountId).ToListAsync();
            return orders.Any(p => p.Items.Any(p => p.JewelryId == jewelryId));
        }

        public Task<Order?> GetDelivererCurrentlyHandledOrder(Account delivererAccount, CancellationToken cancellationToken = default)
        {
            return _set.FirstOrDefaultAsync(x => (x.Status == OrderStatus.Delivering || x.Status == OrderStatus.Prepared) 
            &&x.DelivererId != null && x.DelivererId == delivererAccount.Id);
        }

        public IQueryable<Order> GetDetailQuery(IQueryable<Order> query, bool isIncludeJewelry = true, bool isIncludeDiamond = true)
        {
            query = query
                .Include(p => p.Account);
            if (isIncludeJewelry)
                query = query
                .Include(p => p.Items)
                    .ThenInclude(c => c.Jewelry)
                    .ThenInclude(c => c.Model);
                query = query
                    .Include(p => p.Items)
                        .ThenInclude(c => c.Jewelry)
                        .ThenInclude(c => c.Diamonds)
                        .ThenInclude(c => c.DiamondShape);
            if (isIncludeDiamond)
                query = query
                .Include(p => p.Items)
                    .ThenInclude(c => c.Diamond)
                    .ThenInclude(c => c.DiamondShape);
            return query.AsSplitQuery();
        }
        public bool IsRequestCreated(CustomizeRequestId customizeRequestId)
        {
            return _set.Any(p => p.CustomizeRequestId == customizeRequestId);
        }
    }
}
