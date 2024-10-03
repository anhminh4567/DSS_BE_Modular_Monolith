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
            config.NewConfig<JewelryModel, JewelryModelDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.CategoryId, src => (src.CategoryId == null) ? null : src.CategoryId.Value);

            config.NewConfig<Metal, MetalDto>()
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<MainDiamondReq, MainDiamondReqDto>()
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<MainDiamondShape, MainDiamondShapeDto>()
                .Map(dest => dest.MainDiamondReqId, src => src.MainDiamondReqId.Value)
                .Map(dest => dest.ShapeId, src => src.ShapeId.Value);

            config.NewConfig<SideDiamondOpt, SideDiamondOptDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.SideDiamondReqId, src => src.SideDiamondReqId.Value);

            config.NewConfig<SideDiamondReq, SideDiamondReqDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.ModelId, src => src.ModelId.Value)
                .Map(dest => dest.ShapeId, src => src.ShapeId.Value);

            config.NewConfig<JewelryModelCategory, JewelryModelCategoryDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.ParentCategoryId, src => src.ParentCategoryId.Value);

        }
    }
}
