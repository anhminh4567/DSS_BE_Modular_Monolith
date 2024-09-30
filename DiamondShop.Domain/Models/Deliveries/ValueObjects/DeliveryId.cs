using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Deliveries.ValueObjects
{
    public record DeliveryId(string Value)
    {
        public static DeliveryId Parse(string id)
        {
            return new DeliveryId(id) { Value = id };
        }
        public static DeliveryId Create()
        {
            return new DeliveryId(Guid.NewGuid().ToString());
        }
    }
}
