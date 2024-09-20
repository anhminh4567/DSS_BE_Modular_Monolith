using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record SideDiamondId(string Value)
    {
        public static SideDiamondId Parse(string id)
        {
            return new SideDiamondId(id) { Value = id };
        }
        public static SideDiamondId Create(string id = null)
        {
            if (id != null)
            {
                return new SideDiamondId(id);
            }
            return new SideDiamondId(Guid.NewGuid().ToString());
        }
    }
}
