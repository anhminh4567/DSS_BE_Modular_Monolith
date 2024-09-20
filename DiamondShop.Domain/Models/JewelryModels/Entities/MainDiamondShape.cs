using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class MainDiamondShape
    {
        public MainDiamondId MainDiamondId { get; set; }
        public MainDiamond MainDiamond { get; set; }
        public DiamondShapeId DiamondShapeId { get; set; }
        public DiamondShape DiamondShape { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
    }
}
