using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Models.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Repositories.OrderRepo
{
    public interface IOrderLogRepository : IBaseRepository<OrderLog>
    {
        Task<List<OrderLog>> GetOrderLogs(Order order,CancellationToken cancellationToken = default);
        Task<List<OrderLog>> GetCompleteOrderLogByDateRange(DateTime startDate, DateTime endDate,CancellationToken cancellationToken = default); 
    }
}
