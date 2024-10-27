using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.Discounts.Commands.CreateFull;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class PromotionMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Promotion, PromotionDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.StartDate, src => src.StartDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat))
                .Map(dest => dest.EndDate, src => src.EndDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat));

            config.NewConfig<PromoReq, RequirementDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.PromotionId, src => (src.PromotionId == null) ? null : src.PromotionId.Value)
                .Map(dest => dest.DiscountId, src => (src.DiscountId == null) ? null : src.DiscountId.Value)
                .Map(dest => dest.ModelId, src => (src.ModelId == null) ? null : src.ModelId.Value);

            config.NewConfig<PromoReqShape, RequirementShapeDto>()
                .Map(dest => dest.PromoReqId, src => src.PromoReqId.Value)
                .Map(dest => dest.ShapeId, src => src.ShapeId.Value);

            config.NewConfig<Gift, GiftDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.PromotionId, src => (src.PromotionId == null) ? null : src.PromotionId.Value)
                .Map(dest => dest.DiamondGiftShapes, src => src.DiamondGiftShapes.Select(x => x.Value));

            config.NewConfig<Discount, DiscountDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.StartDate, src => src.StartDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat))
                .Map(dest => dest.EndDate, src => src.EndDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat));

            // this is mapping from request to command
            config.NewConfig<DiscountRequirement, RequirementSpec>()
                .Map(dest => dest.DiamondRequirementSpec, src => src.DiamondRequirementSpec)
                .Map(dest => dest.DiamondRequirementSpec.cutFrom, src => src.DiamondRequirementSpec.cutFrom,src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.cutTo, src => src.DiamondRequirementSpec.cutTo, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.colorFrom, src => src.DiamondRequirementSpec.colorFrom, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.colorTo, src => src.DiamondRequirementSpec.colorTo, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.clarityFrom, src => src.DiamondRequirementSpec.clarityFrom, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.clarityTo, src => src.DiamondRequirementSpec.clarityTo, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.caratFrom, src => src.DiamondRequirementSpec.caratFrom, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.DiamondRequirementSpec.caratTo, src => src.DiamondRequirementSpec.caratTo, src => src.DiamondRequirementSpec != null)
                .Map(dest => dest.MoneyAmount, src=> 10000.0m)
                .Map(dest => dest.Quantity, src=> 1)
                .Map(dest => dest.JewelryModelID, src => src.JewelryModelID)
                .Map(dest => dest.isPromotion, src => false);

        }
    }
}
