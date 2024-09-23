using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record SizeId(string Value)
    {
        public static SizeId Parse(string id)
        {
            return new SizeId(id) { Value = id };
        }
        public static SizeId Create()
        {
            return new SizeId(Guid.NewGuid().ToString());
        }
    }
}
