using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class JewelryModelMedia : Entity<JewelryModelMediaId>
    {
        public JewelryModelId ModelId { get; set; }
        public MetalId MetalId { get; set; }
        public string MediaName { get; set; }
        public string MediaPath { get; set; }
        public MainDiamond MainDiamond { get; set; }
    }
}
