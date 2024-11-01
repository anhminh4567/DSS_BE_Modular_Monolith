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
            var getshapechars = shape.Shape.Substring(0, 2);
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
        public decimal BiggestSideDiamondCarat { get; set; } = 0.2M;
        public decimal AverageOffsetVeryGoodCutFromExcelentCut { get; set; } = -0.11M;
        public decimal AverageOffsetGoodCutFromExcelentCut { get; set; } = -0.15M;
        public decimal AverageOffsetFancyShapeFromRoundShape { get; set; } = -0.2M;
    }
}
