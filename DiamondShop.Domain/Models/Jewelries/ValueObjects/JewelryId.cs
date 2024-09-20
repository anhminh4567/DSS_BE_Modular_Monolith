using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.ValueObjects
{
    public record JewelryId ( string Value)
    {
        public static JewelryId Parse(string id)
        {
            return new JewelryId(id) { Value = id };
        }
        public static JewelryId Create()
        {
            return new JewelryId(Guid.NewGuid().ToString());
        }
    }
}
