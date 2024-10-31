using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSelling;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetSellingDetail;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using Mapster;

namespace DiamondShop.Application.Mappers
{
    public class JewelryModelMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<JewelryModel, JewelryModelDto>();

            config.NewConfig<Metal, MetalDto>();

            config.NewConfig<MainDiamondReq, MainDiamondReqDto>();

            config.NewConfig<MainDiamondShape, MainDiamondShapeDto>();

            config.NewConfig<SideDiamondOpt, SideDiamondOptDto>();

            config.NewConfig<JewelryModelCategory, JewelryModelCategoryDto>();

            config.NewConfig<JewelryModelSelling, JewelryModelSellingDto>();

            config.NewConfig<JewelryModelSellingDetail, JewelryModelSellingDetailDto>();

        }
    }
}
