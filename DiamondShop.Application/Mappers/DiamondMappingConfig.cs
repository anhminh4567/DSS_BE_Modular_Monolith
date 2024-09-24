using DiamondShop.Application.Commons.Constants;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Entities;
using DiamondShop.Domain.Models.DiamondShapes;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class DiamondMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Diamond, DiamondDto>()
                .Map(dest => dest.DiamondShapeId, src => src.DiamondShapeId.Value)
                .Map(dest => dest.JewelryId, src => src.JewelryId.Value);

            config.NewConfig<DiamondShape, DiamondShapeDto>()
                .Map(dest => dest.ShapeName, src => src.Shape);
            config.NewConfig<DiamondWarranty, DiamondWarrantyDto>()
                .Map(dest => dest.CreatedDate, src => src.CreatedDate.ToString(DateTimeResponseFormat.DATE_TIME_FORMAT))
                .Map(dest => dest.ExpiredDate, src => src.ExpiredDate.ToString(DateTimeResponseFormat.DATE_TIME_FORMAT))
                .Map(dest => dest.EffectiveDate, src => src.EffectiveDate.ToString(DateTimeResponseFormat.DATE_TIME_FORMAT))
                .Map(dest => dest.WarrantyType, src => src.WarrantyType.ToString());


        }
    }
}
