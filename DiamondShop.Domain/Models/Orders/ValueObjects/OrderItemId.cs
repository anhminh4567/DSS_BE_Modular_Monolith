using DiamondShop.Domain.Models.JewelryModels.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record OrderItemId(string Value)
    {
        public static OrderItemId Parse(string id)
        {
            return new OrderItemId(id) { Value = id };
        }
        public static OrderItemId Create()
        {
            return new OrderItemId(Guid.NewGuid().ToString());
        }
    }
}
