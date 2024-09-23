using DiamondShop.Domain.Models.DiamondShapes.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record JewelryModelId ( string Value)
    {
        public static JewelryModelId Parse(string id)
        {
            return new JewelryModelId(id) { Value = id };
        }
        public static JewelryModelId Create()
        {
            return new JewelryModelId(Guid.NewGuid().ToString());
        }
    }
}
