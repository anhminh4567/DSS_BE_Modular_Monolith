using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels;
using DiamondShop.Domain.Models.JewelryModels.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }
    }
}
