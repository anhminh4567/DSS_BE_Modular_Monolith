using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.ValueObjects
{
    public record JewelryReviewMediaId (string Value)
    {
        public static JewelryReviewMediaId Parse(string id)
        {
            return new JewelryReviewMediaId(id) { Value = id };
        }
        public static JewelryReviewMediaId Create()
        {
            return new JewelryReviewMediaId(Guid.NewGuid().ToString());
        }
    }
}
