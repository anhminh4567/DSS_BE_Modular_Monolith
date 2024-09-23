using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class SideDiamondOpt : Entity<SideDiamondOptId>
    {
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
        public SideDiamondReqId SideDiamondReqId { get; set; }
        public SideDiamondReq SideDiamondReq { get; set; }
        public SideDiamondOpt() { }
    }
}
