using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Jewelries.ValueObjects
{
    public record JewelryReviewId(string Value)
    {
        public static JewelryReviewId Parse(string id)
        {
            return new JewelryReviewId(id) { Value = id };
        }
        public static JewelryReviewId Create()
        {
            return new JewelryReviewId(Guid.NewGuid().ToString());
        }
    }
}
