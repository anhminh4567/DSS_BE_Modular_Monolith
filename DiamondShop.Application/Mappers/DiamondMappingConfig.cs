using DiamondShop.Application.Commons;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
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
                .Map(dest => dest.JewelryId, src => src.JewelryId.Value)
                .Map(dest => dest.Id, src => src.Id.Value);


            config.NewConfig<DiamondShape, DiamondShapeDto>()
                .Map(dest => dest.ShapeName, src => src.Shape)
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<DiamondWarranty, DiamondWarrantyDto>()
                .Map(dest => dest.CreatedDate, src => src.CreatedDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat))
                .Map(dest => dest.ExpiredDate, src => src.ExpiredDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat))
                .Map(dest => dest.EffectiveDate, src => src.EffectiveDate.ToLocalTime().ToString(DateTimeFormatingRules.DateTimeFormat))
                .Map(dest => dest.WarrantyType, src => src.WarrantyType.ToString());

            config.NewConfig<DiamondCriteria, DiamondCriteriaDto>()
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<DiamondPrice, DiamondPriceDto>()
                .Map(dest => dest.ShapeId, src => src.ShapeId.Value)
                .Map(dest => dest.CriteriaId, src => src.CriteriaId.Value);

        }
    }
}
