using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DiamondPrices.ValueObjects;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class GeneralTypeMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Dictionary<SideDiamondReqId, SideDiamondOptId>, Dictionary<string, string>>()
                .MapWith( src => src.ToDictionary(kvp => kvp.Key.Value.ToString(), kvp => kvp.Value.Value.ToString()));
            config.NewConfig<JewelryId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiamondId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiamondCriteriaId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiamondShapeId, string>()
               .MapWith(src => src.Value);

            config.NewConfig<JewelryId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<JewelryModelId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<SizeId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<MetalId, string>()
               .MapWith(src => src.Value);
           
            config.NewConfig<PromotionId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<PromoReqId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<GiftId, string>()
               .MapWith(src => src.Value);
            config.NewConfig<DiscountId, string>()
                   .MapWith(src => src.Value);
        }
    }
}
