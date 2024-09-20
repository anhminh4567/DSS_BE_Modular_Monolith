using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record MainDiamondId(string Value)
    {
        public static MainDiamondId Parse(string id)
        {
            return new MainDiamondId(id) { Value = id };
        }
        public static MainDiamondId Create()
        {
            return new MainDiamondId(Guid.NewGuid().ToString());
        }
    }
}
