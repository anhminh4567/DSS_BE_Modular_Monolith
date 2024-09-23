using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ValueObjects
{
    public record DiscountId(string Value)
    {
        public static DiscountId Parse(string id)
        {
            return new DiscountId(id) { Value = id };
        }
        public static DiscountId Create()
        {
            return new DiscountId(Guid.NewGuid().ToString());
        }
    }
}
