using DiamondShop.Domain.Models.CustomizeRequests.ValueObjects;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.Entities
{
    public class SideDiamondRequest
    {
        public SideDiamondReqId SideDiamondReqId { get; set; }
        public CustomizeRequestId CustomizeRequestId { get; set; }
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
    }
}
