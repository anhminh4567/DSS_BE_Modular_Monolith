using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.Entities
{
    public class Metal : Entity<MetalId>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Metal() { }
        public void Update(decimal price) => Price = price;
    }
}
