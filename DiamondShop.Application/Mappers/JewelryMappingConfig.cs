using DiamondShop.Application.Dtos.Responses.Diamonds;
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
                .Map(dest => dest.SideDiamond, src => src.SideDiamond)
                .Map(dest => dest.ModelCode, src => src.Model != null ? src.Model.ModelCode : null)
                .Map(dest => dest.Category, src => src.Model != null ? src.Model.Category : null)
                ;

            config.NewConfig<JewelryReview, JewelryReviewDto>();

            config.NewConfig<JewelrySideDiamond, JewelrySideDiamondDto>();

            config.NewConfig<Jewelry, JewelryDiamondDto>();
        }
    }
}
