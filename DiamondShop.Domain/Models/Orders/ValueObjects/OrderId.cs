using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record OrderId (string Value)
    {
        public static OrderId Parse(string id)
        {
            return new OrderId(id) { Value = id };
        }
        public static OrderId Create()
        {
            return new OrderId(Guid.NewGuid().ToString());
        }
    }
}
