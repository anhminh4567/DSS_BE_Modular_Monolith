using DiamondShop.Domain.Models.DiamondShapes;
using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.Entities
{
    public class PromoReqShape
    {
        public PromoReqId PromoReqId { get; set; }
        public PromoReq PromoReq { get; set; }
        public DiamondShapeId DiamondShapeId { get; set; }
        public DiamondShape DiamondShape { get; set; }
    }
}
