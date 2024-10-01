using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.ValueObjects
{
    public record CartItemId(string Value)
    {
        public static CartItemId Parse(string id)
        {
            return new CartItemId(id) { Value = id };
        }
        public static CartItemId Create()
        {
            return new CartItemId(DateTime.UtcNow.Ticks.ToString());
        }
    }
}
