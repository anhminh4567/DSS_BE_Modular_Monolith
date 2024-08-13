using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomerAggregate.ValueObjects
{
    public record CustomerId(string Value)
    {
        public static CustomerId Parse(string id)
        {
            return new CustomerId(id);
        }
        public static CustomerId Create()
        {
            return new CustomerId(Guid.NewGuid().ToString());
        }
    }

}
