using DiamondShop.Domain.Models.Transactions.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Promotions.ValueObjects
{
    public record PromotionId(string Value)
    {
        public static PromotionId Parse(string id)
        {
            return new PromotionId(id) { Value = id };
        }
        public static PromotionId Create()
        {
            return new PromotionId(Guid.NewGuid().ToString());
        }
    }
}
