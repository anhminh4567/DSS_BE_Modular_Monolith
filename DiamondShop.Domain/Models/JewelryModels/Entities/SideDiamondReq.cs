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
    public class SideDiamondReq: Entity<SideDiamondReqId>
    {
        public SideDiamondId SideDiamondId { get; set; }
        public DiamondShapeId DiamondShapeId { get; set; }
        public DiamondShape DiamondShape { get; set; }
        public SettingType SettingType { get; set; }
        public Color ColorMin { get; set; }
        public Color ColorMax { get; set; }
        public Clarity ClarityMin { get; set; }
        public Clarity ClarityMax { get; set; }
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
    }
}
