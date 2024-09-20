using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class SideDiamond : Entity<SideDiamondId>
    {
        public JewelryModelId JewelryModelId { get; set; }
        public List<SideDiamondReq> SideDiamondReqs { get; set; } = new ();
    }
}
