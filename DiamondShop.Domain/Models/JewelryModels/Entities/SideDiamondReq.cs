using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.Enums;
using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.Enum;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public record SideDiamondSpec(string ShapeId, Color ColorMin, Color ColorMax, Clarity ClarityMin, Clarity ClarityMax, SettingType SettingType, List<SideDiamondOptSpec> OptSpecs, string ModelId = null);
    public class SideDiamondReq : Entity<SideDiamondReqId>
    {
        public JewelryModelId ModelId { get; set; }
        public JewelryModel Model { get; set; }
        public DiamondShapeId ShapeId { get; set; }
        public DiamondShape Shape { get; set; }
        public Color ColorMin { get; set; }
        public Color ColorMax { get; set; }
        public Clarity ClarityMin { get; set; }
        public Clarity ClarityMax { get; set; }
        public SettingType SettingType { get; set; }
        public List<SideDiamondOpt> SideDiamondOpts { get; set; } = new();
        public SideDiamondReq() { }
        public static SideDiamondReq Create(JewelryModelId modelId, DiamondShapeId shapeId, Color colorMin, Color colorMax, Clarity clarityMin, Clarity clarityMax, SettingType settingType, SideDiamondReqId givenId = null)
        {
            return new SideDiamondReq()
            {
                Id = givenId is null? SideDiamondReqId.Create() : givenId,
                ModelId = modelId,
                ShapeId = shapeId,
                ColorMin = colorMin,
                ColorMax = colorMax,
                ClarityMin = clarityMin,
                ClarityMax = clarityMax,
                SettingType = settingType,
            };
        }
    }
}
