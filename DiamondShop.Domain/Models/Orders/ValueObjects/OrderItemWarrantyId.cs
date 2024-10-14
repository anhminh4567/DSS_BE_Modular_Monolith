using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record OrderItemWarrantyId(string Value)
    {
        public static OrderItemWarrantyId Parse(string id)
        {
            return new OrderItemWarrantyId(id) { Value = id };
        }
        public static OrderItemWarrantyId Create()
        {
            return new OrderItemWarrantyId(Guid.NewGuid().ToString());
        }
    }
}
