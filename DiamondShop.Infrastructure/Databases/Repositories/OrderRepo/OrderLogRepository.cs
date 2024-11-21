using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Databases.Repositories.OrderRepo
{
    internal class OrderLogRepository : BaseRepository<OrderLog>, IOrderLogRepository
    {
        public OrderLogRepository(DiamondShopDbContext dbContext) : base(dbContext) { }

        public Task<List<OrderLog>> GetCompleteOrderLogByDateRange(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var startUtc = startDate.ToUniversalTime();
            var endUtc = endDate.ToUniversalTime();
            var query = _set.AsQueryable()
                .Where(x => x.CreatedDate >= startUtc && x.CreatedDate <= endUtc && x.Status == OrderStatus.Success);
            return query.ToListAsync(cancellationToken);
        }

        public Task<List<OrderLog>> GetOrderLogs(Order order,CancellationToken cancellationToken = default)
        {
            var orderId = order.Id;
            return _set.Where(x => x.OrderId == orderId).ToListAsync(cancellationToken);
        }
    }
}
