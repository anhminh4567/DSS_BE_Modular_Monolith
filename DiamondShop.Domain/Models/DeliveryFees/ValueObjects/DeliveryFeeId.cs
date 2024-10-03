using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.DeliveryFees.ValueObjects
{
    public record DeliveryFeeId(string Value)
    {

        public static DeliveryFeeId Parse(string id)
        {
            return new DeliveryFeeId(id) { Value = id };
        }
        public static DeliveryFeeId Create()
        {
            return new DeliveryFeeId(Guid.NewGuid().ToString());
        }
    }
}
