using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    public class GeneralTypeMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Dictionary<SideDiamondReqId, SideDiamondOptId>, Dictionary<string, string>>()
                .Map(dest => dest, src => src.ToDictionary(kvp => kvp.Key.Value.ToString(), kvp => kvp.Value.Value.ToString()));//.Compile();
            config.NewConfig<JewelryId, string>()
               .Map(dest => dest, src => src.Value).Compile();
            config.NewConfig<DiamondId, string>()
               .Map(dest => dest, src => src.Value).Compile();
            config.NewConfig<JewelryModelId, string>()
               .Map(dest => dest, src => src.Value).Compile();
            config.NewConfig<SizeId, string>()
               .Map(dest => dest, src => src.Value).Compile();
            config.NewConfig<MetalId, string>()
               .Map(dest => dest, src => src.Value).Compile();
        }
    }
}
