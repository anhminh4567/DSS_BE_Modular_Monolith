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
            config.NewConfig<Jewelry, JewelryDto>();

            config.NewConfig<JewelryReview, JewelryReviewDto>();

            config.NewConfig<JewelrySideDiamond, JewelrySideDiamondDto>();
        }
    }
}
