using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Diamonds.ValueObjects
{
    public record DiamondId(string Value)
    {
        public static DiamondId Parse(string id)
        {
            return new DiamondId(id) { Value = id };
        }
        public static DiamondId Create()
        {
            return new DiamondId(Guid.NewGuid().ToString());
        }
    }
}
