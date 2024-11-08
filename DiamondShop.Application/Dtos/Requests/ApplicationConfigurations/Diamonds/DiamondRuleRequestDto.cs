using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.ApplicationConfigurations.Diamonds
{
    public class DiamondRuleRequestDto
    {
        public decimal? MinPriceOffset { get; set; }
        public decimal? MaxPriceOffset { get; set; }
        public float? BiggestSideDiamondCarat { get; set; }
        public float? SmallestMainDiamondCarat { get; set; }
        public int? MainDiamondMaxFractionalNumber { get; set; }
        public decimal? AverageOffsetVeryGoodCutFromIdealCut { get; set; }
        public decimal? AverageOffsetGoodCutFromIdealCut { get; set; }
        public decimal? AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE { get; set; }
        public decimal? AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE { get; set; }
        public decimal? PearlOffsetFromFancyShape { get; set; }
        public decimal? PrincessOffsetFromFancyShape { get; set; }
        public decimal? CushionOffsetFromFancyShape { get; set; }
        public decimal? EmeraldOffsetFromFancyShape { get; set; }
        public decimal? OvalOffsetFromFancyShape { get; set; }
        public decimal? RadiantOffsetFromFancyShape { get; set; }
        public decimal? AsscherOffsetFromFancyShape { get; set; }
        public decimal? MarquiseOffsetFromFancyShape { get; set; }
        public decimal? HeartOffsetFromFancyShape { get; set; }
    }
}
