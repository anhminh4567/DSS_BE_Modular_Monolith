using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.ErrorMessages;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.OrderRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetDiscountUsageDetail
{
    public record GetDiscountUsageDetailQuery(string discountId, bool? includeOrderCount = true, bool? includeOrderPrices = false, bool? includeOrderIds = false) : IRequest<Result<DiscountUsageDetailResponseDto>>;
    internal class GetDiscountUsageDetailQueryHandler : IRequestHandler<GetDiscountUsageDetailQuery, Result<DiscountUsageDetailResponseDto>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IMapper _mapper;

        public GetDiscountUsageDetailQueryHandler(IDiscountRepository discountRepository, IOrderRepository orderRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _orderRepository = orderRepository;
            _optionsMonitor = optionsMonitor;
            _mapper = mapper;
        }

        public async Task<Result<DiscountUsageDetailResponseDto>> Handle(GetDiscountUsageDetailQuery request, CancellationToken cancellationToken)
        {
            var parsedId = PromotionId.Parse(request.discountId);
            var discount = await _discountRepository.GetById(parsedId);
            if (discount == null)
                return Result.Fail(PromotionError.NotFound);
            var response = new DiscountUsageDetailResponseDto();
            response.Discount= _mapper.Map<DiscountDto>(discount);
            if (request.includeOrderCount != null && request.includeOrderCount.Value)
            {
                response.TotalUsageFromOrders= await _discountRepository.GetDiscountCountFromOrder(discount, x => Discount.NotCountAsUsed.Contains(x.Status) == false);
            }
            if (request.includeOrderIds != null && request.includeOrderIds.Value)
            {
                var result = await _discountRepository.GetOrderIdsFromOrder(discount, x => Discount.NotCountAsUsed.Contains(x.Status) == false);
                response.OrderIdsUsage = _mapper.Map<List<string>>(result);
            }
            if (request.includeOrderPrices != null && request.includeOrderPrices.Value)
            {
                //TODO: doi migrate lay gia tri tu order va item 
                //response.TotalMoneySpent = await _promotionRepository.GetPromotionMoneySpentOnOrders(x => Promotion.StatusNOTQualifiedAsUsed.Contains(x.Status) == false);
            }
            return response;
        }
    }
}
