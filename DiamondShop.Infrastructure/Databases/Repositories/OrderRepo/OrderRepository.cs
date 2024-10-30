using DiamondShop.Domain.Models.Orders;
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
                        .ThenInclude(c => c.Diamonds);
            if (isIncludeDiamond)
                query = query
                .Include(p => p.Items)
                    .ThenInclude(c => c.Diamond);
            return query.AsSplitQuery();
        }
    }
}
