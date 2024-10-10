using DiamondShop.Application.Services.Data;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using DiamondShop.Infrastructure.Outbox;
using FluentEmail.Core.Interfaces;
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

        public Task Execute(IJobExecutionContext context)
        {
            var query = _promotionRepository.GetQuery();
            var dateTimeNow = DateTime.UtcNow;
            var unstartedPromotion = from promotions in query
                         where promotions.PromoReqs.Count > 0 
                         && promotions.Gifts.Count > 0
                         && promotions.StartDate < dateTimeNow
                         && promotions.EndDate > dateTimeNow
                         && promotions.IsActive == false
                         select promotions;
            return Task.CompletedTask;
        }
    }
}
