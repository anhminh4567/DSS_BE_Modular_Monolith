using DiamondShop.Domain.Common.Ultilities;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class DiamondRule
    {
        public static DiamondRule Default = new DiamondRule();
        public static string Type = typeof(DiamondRule).AssemblyQualifiedName;
        public static string key = "DiamondRuleVer5";

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
        // gia toi thieu sideDiamond, neu tim thay gia, tinh TB 1 vien, ra gia, ma gia nos be hon 
        // gia toi thieu, thi gia se la gia toi thieu
        public decimal MinimalSideDiamondAveragePrice { get; set; } = 1000.0m ;
        public decimal MinimalMainDiamondPrice { get; set; } = 10000.0m;

        public decimal MinPriceOffset{ get; set; } = -0.90m;
        public decimal MaxPriceOffset { get; set; } = +0.90m;
        public decimal MinCaratRange { get; set; } = 0.15m;
        public decimal MaxCaratRange { get; set; } =10m;

        public float BiggestSideDiamondCarat { get; set; } = 0.3f;
        public float SmallestMainDiamondCarat { get; set; } = 0.15f;
        public int MainDiamondMaxFractionalNumber { get; set; } = 2;
        public decimal AverageOffsetVeryGoodCutFromIdealCut { get; set; } = -0.11M;
        public decimal AverageOffsetGoodCutFromIdealCut { get; set; } = -0.15M;
        public decimal AverageOffsetExcellentCutFromIdealCut { get; set; } = -0.02M;

        public int MaxLockTimeForCustomer { get; set; } = 24;
        /// <summary>
        /// 
        /// </summary>
        public decimal AverageOffsetVeryGoodCutFromIdealCut_FANCY_SHAPE { get; set; } = -0.11M;
        public decimal AverageOffsetGoodCutFromIdealCut_FANCY_SHAPE { get; set; } = -0.12M;
        public decimal AverageOffsetExcellentCutFromIdealCut_FANCY_SHAPE { get; set; } = -0.02M;

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
        public decimal? GetFancyShapeOffset(DiamondShape shape)
        {
            if (shape.IsFancy() == false)
                return null;
            var shapeId = shape.Id;
            if(shapeId == DiamondShape.HEART.Id)
                return HeartOffsetFromFancyShape;
            if (shapeId == DiamondShape.RADIANT.Id)
                return RadiantOffsetFromFancyShape;
            if (shapeId == DiamondShape.OVAL.Id)
                return OvalOffsetFromFancyShape;
            if(shapeId == DiamondShape.CUSHION.Id)
                return CushionOffsetFromFancyShape;
            if(shapeId == DiamondShape.MARQUISE.Id)
                return MarquiseOffsetFromFancyShape;
            if(shapeId == DiamondShape.ASSCHER.Id)
                return AsscherOffsetFromFancyShape;
            if(shapeId == DiamondShape.CUSHION.Id)
                return CushionOffsetFromFancyShape;
            if(shapeId == DiamondShape.EMERALD.Id)
                return EmeraldOffsetFromFancyShape;
            if(shapeId == DiamondShape.PRINCESS.Id)
                return PrincessOffsetFromFancyShape;
            return null;
        }
        public decimal? GetCutOffset(Cut cut)
        {
            switch (cut)
            {
                case Cut.Excellent:
                    return AverageOffsetExcellentCutFromIdealCut;
                case Cut.Very_Good:
                    return AverageOffsetVeryGoodCutFromIdealCut;
                case Cut.Good:
                    return AverageOffsetGoodCutFromIdealCut;
                default:
                    return null;
            }
        }
    }
    public class OffsetByShapeAndCut
    {
        public decimal Offset { get; set; }
        public DiamondShape Shape { get; set; }
        public Cut Cut { get; set; }
    }
}
