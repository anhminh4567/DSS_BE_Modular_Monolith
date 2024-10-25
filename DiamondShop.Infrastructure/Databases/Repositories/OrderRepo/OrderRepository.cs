﻿using DiamondShop.Domain.Models.Orders;
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

        public IQueryable<Order> GetDetailQuery(IQueryable<Order> query)
        {
            return query
                .Include(p => p.Account)
                .Include(p => p.Items)
                    .ThenInclude(c => c.Jewelry)
                .Include(p => p.Items)
                    .ThenInclude(c => c.Diamond)
                .AsSplitQuery();
        }
    }
}
