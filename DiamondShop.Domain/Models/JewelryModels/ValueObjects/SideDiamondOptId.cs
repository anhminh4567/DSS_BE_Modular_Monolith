using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record SideDiamondOptId(string Value)
    {
        public static SideDiamondOptId Parse(string id)
        {
            return new SideDiamondOptId(id) { Value = id };
        }
        public static SideDiamondOptId Create(string id = null)
        {
            if (id != null)
            {
                return new SideDiamondOptId(id);
            }
            return new SideDiamondOptId(Guid.NewGuid().ToString());
        }
    }
}
