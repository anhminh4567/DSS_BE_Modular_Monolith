using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record JewelryModelMediaId(string Value)
    {
        public static JewelryModelMediaId Parse(string id)
        {
            return new JewelryModelMediaId(id) { Value = id };
        }
        public static JewelryModelMediaId Create()
        {
            return new JewelryModelMediaId(Guid.NewGuid().ToString());
        }
    }
}
