using DiamondShop.Commons;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Discounts.Queries.GetDetail
{
    public record GetDiscountDetailQuery(string? discountId, string? discountCode) : IRequest<Result<Discount>>;
    internal class GetDiscountDetailQueryHandler : IRequestHandler<GetDiscountDetailQuery, Result<Discount>>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;
        private readonly ILogger<GetDiscountDetailQueryHandler> _logger;

        public GetDiscountDetailQueryHandler(IDiscountRepository discountRepository, IJewelryModelRepository jewelryModelRepository, ILogger<GetDiscountDetailQueryHandler> logger)
        {
            _discountRepository = discountRepository;
            _jewelryModelRepository = jewelryModelRepository;
            _logger = logger;
        }

        public async Task<Result<Discount>> Handle(GetDiscountDetailQuery request, CancellationToken cancellationToken)
        {
            if(request.discountId is null && request.discountCode is null)
            {
                _logger.LogError("id or discountCode is required");
                return Result.Fail(new Error("id or discountCode is required"));
            }
            if(request.discountCode is not null)
            {
                var result = await _discountRepository.GetByCode(request.discountCode);
                if (result is null)
                {
                    _logger.LogError("no discount found");
                    return Result.Fail(new NotFoundError());
                }
                await MapDiscount(result);
                return result;
            }
            else if (request.discountId is not null)
            {
                var reqId = DiscountId.Parse(request.discountId);
                var result = await _discountRepository.GetById(reqId);
                if (result is null)
                {
                    _logger.LogError("no discount found");
                    return Result.Fail(new NotFoundError());
                }
                await MapDiscount(result);
                return result;
            }
            return Result.Fail("Unknown error") ;
        }
        private async Task MapDiscount(Discount discount)
        {
            foreach (var req in discount.DiscountReq)
            {
                if (req.TargetType == TargetType.Jewelry_Model && req.ModelId != null)
                {
                    var model = await _jewelryModelRepository.GetById(req.ModelId);
                    req.Model = model;
                }
            }
        }
    }
}
