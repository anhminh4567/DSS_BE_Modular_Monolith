using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DiamondPrices.ValueObjects
{
    public record DiamondCriteriaId (string Value)
    {
        public static DiamondCriteriaId Parse(string id)
        {
            return new DiamondCriteriaId(id) { Value = id};
        }
        public static DiamondCriteriaId Create()
        {
            return new DiamondCriteriaId(DateTime.UtcNow.Ticks.ToString());
        }
    }
}
