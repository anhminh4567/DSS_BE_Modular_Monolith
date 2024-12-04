using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetPromotionUsageDetail
{
    
    public record GetPromotionUsageDetailQuery(string promotionId,bool? includeOrderCount = true, bool? includeOrderPrices  = false, bool? includeOrderIds = false) : IRequest<Result<PromotionUsageDetailResponseDto>>;
    internal class GetPromotionUsageDetailQueryHandler : IRequestHandler<GetPromotionUsageDetailQuery, Result<PromotionUsageDetailResponseDto>>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly ILogger<GetPromotionUsageDetailQueryHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IMapper _mapper;

        public GetPromotionUsageDetailQueryHandler(IPromotionRepository promotionRepository, ILogger<GetPromotionUsageDetailQueryHandler> logger, IOrderRepository orderRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _logger = logger;
            _orderRepository = orderRepository;
            _optionsMonitor = optionsMonitor;
            _mapper = mapper;
        }

        public async Task<Result<PromotionUsageDetailResponseDto>> Handle(GetPromotionUsageDetailQuery request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.promotionId);
            var promotion= await _promotionRepository.GetById(parsedId);
            if (promotion == null)
                return Result.Fail(PromotionError.NotFound);
            var response = new PromotionUsageDetailResponseDto();
            response.Promotion = _mapper.Map<PromotionDto>(promotion);
            if (request.includeOrderCount != null && request.includeOrderCount.Value)
            {
                response.TotalCurrentUsage = await _promotionRepository.GetPromotionCountFromOrders(promotion,x => Promotion.StatusNOTQualifiedAsUsed.Contains(x.Status) == false);
            }
            if(request.includeOrderIds != null && request.includeOrderIds.Value)
            {
                var result = await _promotionRepository.GetPromotionIdsFromOrders(promotion, x => Promotion.StatusNOTQualifiedAsUsed.Contains(x.Status) == false);
                response.OrderIdsUsage = _mapper.Map<List<string>>(result);
            }
            if(request.includeOrderPrices != null && request.includeOrderPrices.Value)
            {
                //TODO: doi migrate lay gia tri tu order va item 
                //response.TotalMoneySpent = await _promotionRepository.GetPromotionMoneySpentOnOrders(x => Promotion.StatusNOTQualifiedAsUsed.Contains(x.Status) == false);
            }
            return response;
        }
    }
}
