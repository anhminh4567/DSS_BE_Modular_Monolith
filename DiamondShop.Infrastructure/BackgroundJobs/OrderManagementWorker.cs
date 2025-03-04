﻿using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.Enums;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Domain.Services.interfaces;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Databases.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Syncfusion.XlsIO.Implementation.TemplateMarkers;
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
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IPaymentService _paymentService;
        private readonly DiamondShopDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly IOrderLogRepository _orderLogRepository;

        public OrderManagementWorker(IUnitOfWork unitOfWork, ILogger<OrderManagementWorker> logger, IOrderRepository orderRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IPaymentService paymentService, DiamondShopDbContext dbContext, IOrderService orderService, IOrderLogRepository orderLogRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _orderRepository = orderRepository;
            _optionsMonitor = optionsMonitor;
            _paymentService = paymentService;
            _dbContext = dbContext;
            _orderService = orderService;
            _orderLogRepository = orderLogRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await CheckExpiredOrder(context);
        }
        private async Task CheckExpiredOrder(IJobExecutionContext context)
        {
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            var query = _dbContext.Orders.AsQueryable();// _orderRepository.GetQuery();
            DateTime utcNow = DateTime.UtcNow;
            DateTime correctExpiredTime = utcNow.Subtract(TimeSpan.FromHours(orderRule.ExpiredOrderHour));
            query = query.Where(o =>o.Status == Domain.Models.Orders.Enum.OrderStatus.Pending
            && o.CreatedDate <= correctExpiredTime ).Include(x => x.Items).Include(x => x.Transactions);
            var result = query.ToList();

            if (result.Count == 0)
                return;
            _logger.LogInformation("Found {0} order(s) to be expired", result.Count);
            foreach (var order in result)
            {
                order.SetExpired(utcNow);
                foreach (var item in order.Transactions)
                {
                    if(item.Status == Domain.Models.Transactions.Enum.TransactionStatus.Verifying)
                    {
                        item.Invalidate("Hết hạn thanh toán");
                    }
                }
                await _orderService.CancelItems(order);
                await _paymentService.RemoveAllPaymentCache(order);
                var log = OrderLog.CreateByChangeStatus(order, order.Status);
                await _orderLogRepository.Create(log);
                await _orderRepository.Update(order);
            }
            await _unitOfWork.SaveChangesAsync();

        }
        private async Task CheckExpireOrderAtStore(IJobExecutionContext context)
        {
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            var query = _orderRepository.GetQuery();
            DateTime utcNow = DateTime.UtcNow;
            DateTime correctExpiredTime = utcNow.Subtract(TimeSpan.FromDays(orderRule.DaysWaitForCustomerToPay));
            
        }
    }
}