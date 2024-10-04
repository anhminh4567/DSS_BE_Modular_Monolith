using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class MainDiamondShape
    {
        public MainDiamondReqId MainDiamondReqId { get; set; }
        public MainDiamondReq MainDiamondReq { get; set; }
        public DiamondShapeId ShapeId { get; set; }
        public DiamondShape Shape { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
        public MainDiamondShape() { }
        public static MainDiamondShape Create(MainDiamondReqId mainDiamondReqId, DiamondShapeId shapeId, float caratFrom, float caratTo)
        {
            return new MainDiamondShape()
            {
                MainDiamondReqId = mainDiamondReqId,
                ShapeId = shapeId,
                CaratFrom = caratFrom,
                CaratTo = caratTo,
            }; 
        }
    }
}
