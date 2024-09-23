using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.JewelryModels.ValueObjects
{
    public record JewelryModelCategoryId(string Value)
    {
        public static JewelryModelCategoryId Parse(string id)
        {
            return new JewelryModelCategoryId(id) { Value = id };
        }
        public static JewelryModelCategoryId Create()
        {
            return new JewelryModelCategoryId(Guid.NewGuid().ToString());
        }
    }
}
