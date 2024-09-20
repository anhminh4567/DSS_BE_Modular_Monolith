using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record OrderItemDetailId(string Value)
    {
        public static OrderItemDetailId Parse(string id)
        {
            return new OrderItemDetailId(id) { Value = id };
        }
        public static OrderItemDetailId Create()
        {
            return new OrderItemDetailId(Guid.NewGuid().ToString());
        }
    }
}
