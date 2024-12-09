using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Transactions.Entities;
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
            return _set.Include(o => o.Transactions).ThenInclude(x => x.PayMethod)
                .Include(x => x.Account)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Jewelry)
                .Include(o => o.Items) // Include OrderItems again
                    .ThenInclude(oi => oi.Diamond)
                .Include(o => o.Logs)
                .FirstOrDefaultAsync(o => o.Id == (OrderId)ids[0]);
        }
        public async Task<bool> IsOwner(AccountId accountId, JewelryId jewelryId)
        {
            var orders = await _set.Include(p => p.Items).Where(p => p.Status == OrderStatus.Success && p.AccountId == accountId)
                .ToListAsync();
            return orders.Any(p => p.Items.Any(p => p.JewelryId == jewelryId));
        }

        public Task<Order?> GetDelivererCurrentlyHandledOrder(Account delivererAccount, CancellationToken cancellationToken = default)
        {
            return _set.FirstOrDefaultAsync(x => (x.Status == OrderStatus.Delivering || x.Status == OrderStatus.Prepared)
            && x.DelivererId != null && x.DelivererId == delivererAccount.Id && x.HasDelivererReturned == false);
        }

        public IQueryable<Order> GetDetailQuery(IQueryable<Order> query, bool isIncludeJewelry = true, bool isIncludeDiamond = true)
        {
            query = query
                .Include(p => p.Account);
            query = query
                .Include(p => p.Deliverer);
            query = query.Include(p => p.Transactions)
                .ThenInclude(x => x.PayMethod);
            if (isIncludeJewelry)
            {
                query = query
                .Include(p => p.Items)
                    .ThenInclude(c => c.Jewelry)
                    .ThenInclude(c => c.Review);
                query = query
                .Include(p => p.Items)
                    .ThenInclude(c => c.Jewelry)
                    .ThenInclude(c => c.Model);
                query = query
                    .Include(p => p.Items)
                        .ThenInclude(c => c.Jewelry)
                        .ThenInclude(c => c.Diamonds)
                        .ThenInclude(c => c.DiamondShape);
            }
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
        public IQueryable<Order> GetSoldJewelry()
        {
            var query = _set.AsQueryable();
            query = query.Include(p => p.Items).ThenInclude(p => p.Jewelry).ThenInclude(p => p.Metal);
            query = query.Include(p => p.Items).ThenInclude(p => p.Jewelry).ThenInclude(p => p.Model);
            query = query.Where(p => p.Status == OrderStatus.Success);
            query = query.Where(p => p.Items.Any(k => k.JewelryId != null));
            return query.AsSplitQuery();
        }

        public Task<List<Order>> GetUserProcessingOrders(Account customerAccounts)
        {
            return _set.Where(x => x.AccountId == customerAccounts.Id 
            && x.Status != OrderStatus.Success
            && x.Status != OrderStatus.Rejected 
            && x.Status != OrderStatus.Cancelled)
                .ToListAsync();
        }

        public Task<List<Order>> GetUserOrders(Account customerAccount)
        {
            return _set.Where(x => x.AccountId == customerAccount.Id)
                .ToListAsync();
        }

        public Task<Order?> GetOrderByCode(string code)
        {
            return _set.Where(_set => _set.OrderCode == code)
                .Include(x => x.PaymentMethod)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Jewelry)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Diamond)
                .Include(x => x.Account)
                .Include(x => x.Transactions)
                    .ThenInclude(x => x.PayMethod)
                .Include(x => x.Logs)
                .FirstOrDefaultAsync();
        }
    }
}
