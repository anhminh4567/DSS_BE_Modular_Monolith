using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class Size : Entity<SizeId>
    {
        public string Unit { get; set; }
        public float Value { get; set; }
        public Size() { }
    }
}
