using DiamondShop.Domain.BusinessRules;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Mappers
{
    internal class ApplicationSettingMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<DiamondRule, DiamondRule>()
                .Map(dest => dest.MinPriceOffset, src => src.MinPriceOffset)
                .Map(dest => dest.MaxPriceOffset, src => src.MaxPriceOffset)
                .Map(dest => dest.BiggestSideDiamondCarat, src => src.BiggestSideDiamondCarat)
                .Map(dest => dest.SmallestMainDiamondCarat, src => src.SmallestMainDiamondCarat)
                .Map(dest => dest.MainDiamondMaxFractionalNumber, src => src.MainDiamondMaxFractionalNumber)
                .Map(dest => dest.AverageOffsetVeryGoodCutFromIdealCut, src => src.AverageOffsetVeryGoodCutFromIdealCut)
                .Map(dest => dest.AverageOffsetGoodCutFromIdealCut, src => src.AverageOffsetGoodCutFromIdealCut)
                .Map(dest => dest.AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE, src => src.AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE)
                .Map(dest => dest.AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE, src => src.AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE)
                ;
        }
    }
}
