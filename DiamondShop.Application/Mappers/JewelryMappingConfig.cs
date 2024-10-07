using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.Entities;
using Mapster;

namespace DiamondShop.Application.Mappers
{
    public class JewelryMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Jewelry, JewelryDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.ModelId, src => src.ModelId.Value)
                .Map(dest => dest.MetalId, src => src.MetalId.Value)
                .Map(dest => dest.SizeId, src => src.SizeId.Value)
                .Map(dest => dest.ReviewId, src => src.ReviewId.Value);

            config.NewConfig<JewelryReview, JewelryReviewDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.AccountId, src => src.AccountId.Value);

            config.NewConfig<JewelrySideDiamond, JewelrySideDiamondDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.JewelryId, src => src.JewelryId.Value);
        }
    }
}
