using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public record SideDiamondOptSpec(float CaratWeight, int Quantity);
    public class SideDiamondOpt : Entity<SideDiamondOptId>
    {
        public float CaratWeight { get; set; }
        public int Quantity { get; set; }
        public SideDiamondReqId SideDiamondReqId { get; set; }
        public SideDiamondReq SideDiamondReq { get; set; }
        public SideDiamondOpt() { }
        public static SideDiamondOpt Create(SideDiamondReqId sideDiamondReqId, float caratWeight, int quantity, SideDiamondOptId givenId = null)
        {
            return new SideDiamondOpt()
            {
                Id = givenId is null ? SideDiamondOptId.Create() : givenId,
                CaratWeight = caratWeight,
                Quantity = quantity,
                SideDiamondReqId = sideDiamondReqId,
            };
        }
    }
}
