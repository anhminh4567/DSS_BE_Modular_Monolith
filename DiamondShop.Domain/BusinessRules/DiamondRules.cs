using DiamondShop.Domain.Common.Ultilities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.DiamondShapes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public  class DiamondRules
    {
        public static decimal MinPriceOffset { get; set; } = 0.1M;
        public static decimal MaxPriceOffset{ get; set; } = 10M;
        public static decimal BiggestSideDiamondCarat { get; set; } = 0.2M;
    }
    public class DiamondRule
    {
        public static DiamondRule Default = new DiamondRule();
        public static string Type = typeof(DiamondRule).AssemblyQualifiedName;
        public static string key = "DiamondRule";

        public static string GetDiamondSerialCode(Diamond diamond,DiamondShape shape)
        {
            var getshapechars = shape.Shape.Substring(0, 2).ToUpper();
            var caratString = "C"+diamond.Carat.ToString().Replace(".", "");
            var cut = diamond.Cut == null ? "" : $"CU{( (int)diamond.Cut)}";
            var color = $"CO{(int)diamond.Color}";
            var clarity = $"CL{(int)diamond.Clarity}";
            var origin = diamond.IsLabDiamond ? "L" : "N";
            var format = $"D{origin}{getshapechars}_{caratString}{cut}{color}{clarity}_{RandomGenerators.GetRandomString(5)}";
            return format;
        }
        public decimal MinPriceOffset{ get; set; } = 0.1m;
        public decimal MaxPriceOffset { get; set; } = 1.9m;
        public float BiggestSideDiamondCarat { get; set; } = 0.2f;
        public float SmallestMainDiamondCarat { get; set; } = 0.17f;
        public decimal AverageOffsetVeryGoodCutFromIdealCut { get; set; } = -0.11M;
        public decimal AverageOffsetGoodCutFromIdealCut { get; set; } = -0.15M;

        public decimal AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE { get; set; } = -0.11M;
        public decimal AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE { get; set; } = -0.12M;
        //public decimal AverageOffsetFancyShapeFromRoundShape { get; set; } = -0.2M;
        public decimal PearlOffsetFromFancyShape { get; set; } = -0.2m;
        public decimal PrincessOffsetFromFancyShape { get; set; } = +0.2m;
        public decimal CushionOffsetFromFancyShape { get; set; } = -0.2m;
        public decimal EmeraldOffsetFromFancyShape { get; set; } = +0.2m;
        public decimal OvalOffsetFromFancyShape { get; set; } = +0.2m;
        public decimal RadiantOffsetFromFancyShape { get; set; } = -0.2m;
        public decimal AsscherOffsetFromFancyShape { get; set; } = -0.2m;
        public decimal MarquiseOffsetFromFancyShape { get; set; } = -0.2m;
        public decimal HeartOffsetFromFancyShape { get; set; } = -0.2m;
    }
}
