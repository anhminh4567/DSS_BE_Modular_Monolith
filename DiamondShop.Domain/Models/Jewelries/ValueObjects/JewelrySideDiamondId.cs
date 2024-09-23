using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.ValueObjects
{
    public record JewelrySideDiamondId(string Value)
    {
        public static JewelrySideDiamondId Parse(string id)
        {
            return new JewelrySideDiamondId(id) { Value = id };
        }
        public static JewelrySideDiamondId Create()
        {
            return new JewelrySideDiamondId(Guid.NewGuid().ToString());
        }
    }
}
