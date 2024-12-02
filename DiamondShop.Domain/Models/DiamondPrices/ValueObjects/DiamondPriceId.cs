using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices.ValueObjects
{
    public record DiamondPriceId(string Value)
    {
        public static DiamondPriceId Parse(string id)
        {
            return new DiamondPriceId(id) { Value = id };
        }
        public static DiamondPriceId Create()
        {
            return new DiamondPriceId(Guid.NewGuid().ToString());
        }
    }
}
