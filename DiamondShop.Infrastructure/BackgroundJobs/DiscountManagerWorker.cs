using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Infrastructure.Databases.Repositories.PromotionsRepo;
using MediatR;
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
    internal class DiscountManagerWorker : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DiscountManagerWorker> _logger;
        private readonly ISender _sender;
        private readonly IDiscountRepository _discountRepository;

        public DiscountManagerWorker(IUnitOfWork unitOfWork, ILogger<DiscountManagerWorker> logger, ISender sender, IDiscountRepository promotionRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sender = sender;
            _discountRepository = promotionRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ActivateDiscount(context);
            await EndDiscount(context);
        }
        private async Task ActivateDiscount(IJobExecutionContext context)
        {
            var query = _discountRepository.GetQuery();
            var dateTimeNow = DateTime.UtcNow;
            var unstartedDiscount = await(from discounts in query
                                          where discounts.DiscountReq.Count > 0
                                          && discounts.StartDate < dateTimeNow
                                          && discounts.EndDate > dateTimeNow
                                          && discounts.Status == Domain.Models.Promotions.Enum.Status.Scheduled
                                          select discounts).Include(d => d.DiscountReq).ToListAsync();
            foreach (var discount in unstartedDiscount)
            {
                var result = discount.SetActive();
                if (result.IsSuccess)
                    _discountRepository.Update(discount).GetAwaiter().GetResult();
                else
                    _logger.LogError($"discount with id: {discount.Id.Value} and name: {discount.Name} is error when set active, check code");
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task EndDiscount(IJobExecutionContext context)
        {
            var query = _discountRepository.GetQuery();
            var dateTimeNow = DateTime.UtcNow;
            var endedDiscount = await (from discounts in query
                                           where discounts.DiscountReq.Count > 0
                                           && discounts.EndDate <= dateTimeNow
                                           && ( discounts.Status == Status.Active || discounts.Status == Status.Paused)
                                       select discounts).ToListAsync();
            foreach (var discount in endedDiscount)
            {
                discount.Expired();
                _logger.LogError($"discount with id: {discount.Id.Value} and name: {discount.Name} is expired with end date: {discount.EndDate.ToString(DateTimeFormatingRules.DateTimeFormat)} at time: {DateTime.UtcNow.ToString(DateTimeFormatingRules.DateTimeFormat)}");
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
