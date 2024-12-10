using DiamondShop.Commons;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Repositories.JewelryModelRepo;
using DiamondShop.Domain.Repositories.PromotionsRepo;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Promotions.Queries.GetDetail
{
    public record GetPromotionDetailQuery (string? id, string? promoCode ) : IRequest<Result<Promotion>>;
    internal class GetPromotionDetailQueryHandler : IRequestHandler<GetPromotionDetailQuery, Result<Promotion>>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IJewelryModelRepository _jewelryModelRepository;

        public GetPromotionDetailQueryHandler(IPromotionRepository promotionRepository, IJewelryModelRepository jewelryModelRepository)
        {
            _promotionRepository = promotionRepository;
            _jewelryModelRepository = jewelryModelRepository;
        }

        public async Task<Result<Promotion>> Handle(GetPromotionDetailQuery request, CancellationToken cancellationToken)
        {
            if (request.id is null && request.promoCode is null)
                return Result.Fail(new Error("id or promoCode is required"));
            if(request.promoCode is not null)
            {
                var result = await _promotionRepository.GetByCode(request.promoCode);
                if (result is null)
                    return Result.Fail(new NotFoundError("no promotion found"));
                await MapModel(result);
                return result;
            }
            else if(request.id is not null)
            {
                var parsedId = PromotionId.Parse(request.id);
                var result = await _promotionRepository.GetById(parsedId);
                if (result is null)
                    return Result.Fail(new NotFoundError("no promotion found"));
                await MapModel(result);
                return result;
            }
            return Result.Fail("unkonwn error");
        }
        private async Task MapModel(Promotion promotion)
        {
            foreach(var req in promotion.PromoReqs)
            {
                if(req.TargetType == Domain.Models.Promotions.Enum.TargetType.Jewelry_Model && req.ModelId != null)
                {
                    var model = await _jewelryModelRepository.GetById(req.ModelId);
                    req.Model = model;
                }
            }
            foreach(var gift in promotion.Gifts)
            {
                if(gift.TargetType == Domain.Models.Promotions.Enum.TargetType.Jewelry_Model && gift.ItemCode!= null)
                {
                    //var modelId = JewelryModelId.Parse(gift.ItemCode);
                    var model = await _jewelryModelRepository.GetByCode(gift.ItemCode);
                    gift.GiftedModel = model;
                }
            }
        }
    }
}
