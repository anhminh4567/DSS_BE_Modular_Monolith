using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    public class OrderManagementWorker : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderManagementWorker> _logger;
        private readonly IOrderRepository _orderRepository;
        public OrderManagementWorker(IUnitOfWork unitOfWork, ILogger<OrderManagementWorker> logger, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CheckExpiredOrder(context);
        }
        private async Task CheckExpiredOrder(IJobExecutionContext context)
        {
            var query = _orderRepository.GetQuery();
            DateTime utcNow = DateTime.UtcNow;
            DateTime correctExpiredTime = utcNow.Subtract(TimeSpan.FromHours(OrderRules.ExpiredOrderHour));
            query = query.Where(o => o.ExpiredDate == null
            && o.CancelledDate == null
            && o.CancelledReason == null
            && o.ShippedDate == null
            && o.Status == Domain.Models.Orders.Enum.OrderStatus.Pending
            && o.CreatedDate <= correctExpiredTime );
            var result = query.ToList();
            if (result.Count == 0)
                return;
            foreach(var order in result)
            {
                order.ExpiredDate = utcNow;
                _orderRepository.Update(order).Wait();
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}