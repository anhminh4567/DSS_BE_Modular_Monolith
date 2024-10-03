using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.ValueObjects
{
    public record DeliveryPackageId (string Value )
    {
        public static DeliveryPackageId Parse(string id)
        {
            return new DeliveryPackageId(id) { Value = id };
        }
        public static DeliveryPackageId Create()
        {
            return new DeliveryPackageId(Guid.NewGuid().ToString());
        }
    }
}
