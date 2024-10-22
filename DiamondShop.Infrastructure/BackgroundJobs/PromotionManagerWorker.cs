using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    internal class PromotionManagerWorker : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PromotionManagerWorker> _logger;
        private readonly ISender _sender;
        private readonly IPromotionRepository _promotionRepository;

        public PromotionManagerWorker(IUnitOfWork unitOfWork, ILogger<PromotionManagerWorker> logger, ISender sender, IPromotionRepository promotionRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _sender = sender;
            _promotionRepository = promotionRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await StartPromotion(context);
            await ExpirePromotion(context);
        }
        public async Task StartPromotion(IJobExecutionContext jobExecutionContext)
        {
            var query = _promotionRepository.GetQuery();
            var dateTimeNow = DateTime.UtcNow;
            var unstartedPromotion = await (from promotions in query
                                            where promotions.PromoReqs.Count > 0
                                            && promotions.Gifts.Count > 0
                                            && promotions.StartDate < dateTimeNow
                                            && promotions.EndDate > dateTimeNow
                                            && promotions.Status == Domain.Models.Promotions.Enum.Status.Scheduled
                                            select promotions).Include(d => d.PromoReqs).ToListAsync();
            foreach (var promotion in unstartedPromotion)
            {
                var result = promotion.SetActive();
                if (result.IsSuccess)
                    _promotionRepository.Update(promotion).GetAwaiter().GetResult();
                else
                    _logger.LogError($"promotion with id: {promotion.Id.Value} and name: {promotion.Name} is error when set active, check code");
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ExpirePromotion(IJobExecutionContext jobExecutionContext)
        {
            var query = _promotionRepository.GetQuery();
            var dateTimeNow = DateTime.UtcNow;
            var endedPromotion = await (from promotions in query
                                            where promotions.PromoReqs.Count > 0
                                            && promotions.Gifts.Count > 0
                                            && promotions.EndDate <= dateTimeNow
                                            && (promotions.Status == Status.Active || promotions.Status == Status.Paused)
                                            select promotions).ToListAsync();
            foreach (var promotion in endedPromotion)
            {
                promotion.Expired();
                _logger.LogError($"promotion with id: {promotion.Id.Value} and name: {promotion.Name} is expired ");
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
