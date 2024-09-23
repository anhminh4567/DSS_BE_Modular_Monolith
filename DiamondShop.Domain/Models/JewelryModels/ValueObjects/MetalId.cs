using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record MetalId(string Value)
    {
        public static MetalId Parse(string id)
        {
            return new MetalId(id) { Value = id };
        }
        public static MetalId Create()
        {
            return new MetalId(Guid.NewGuid().ToString());
        }
    }
}
