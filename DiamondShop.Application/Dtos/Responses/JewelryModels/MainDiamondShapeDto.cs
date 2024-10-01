using DiamondShop.Application.Dtos.Responses.Diamonds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Responses.JewelryModels
{
    public class MainDiamondShapeDto
    {
        public string MainDiamondReqId { get; set; }
        public MainDiamondReqDto MainDiamondReq { get; set; }
        public string ShapeId { get; set; }
        public DiamondShapeDto Shape { get; set; }
        public float CaratFrom { get; set; }
        public float CaratTo { get; set; }
    }
}
