using DiamondShop.Application.Commons;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.DiamondPrices;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using Mapster;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
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
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.DiamondPrice, src => src.DiamondPrice);


            config.NewConfig<DiamondShape, DiamondShapeDto>()
                .Map(dest => dest.ShapeName, src => src.Shape)
                .Map(dest => dest.Id, src => src.Id.Value);

            config.NewConfig<DiamondCriteria, DiamondCriteriaDto>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.ShapeId, src => src.ShapeId.Value);

            config.NewConfig<DiamondPrice, DiamondPriceDto>()
                //.Map(dest => dest.ShapeId, src => src.ShapeId.Value)
                .Map(dest => dest.CriteriaId, src => src.CriteriaId.Value);

        }
    }
}
