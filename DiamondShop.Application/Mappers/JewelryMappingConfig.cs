using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail;
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
                .Map(dest => dest.Name, src => src.Model != null ? src.Model.Name : null);

            config.NewConfig<JewelryReview, JewelryReviewDto>();

            config.NewConfig<JewelrySideDiamond, JewelrySideDiamondDto>();
        }
    }
}
