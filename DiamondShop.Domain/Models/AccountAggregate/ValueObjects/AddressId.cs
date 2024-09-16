using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.AccountAggregate.ValueObjects
{
    public record AddressId (string Value)
    {
        public static AddressId Parse(string id)
        {
            return new AddressId(id) { Value = id };
        }
        public static AddressId Create()
        {
            return new AddressId(DateTime.UtcNow.Ticks.ToString());
        }
    }
}
