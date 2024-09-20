using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Transactions.ValueObjects
{
    public record PaymentMethodId(string Value)
    {
        public static PaymentMethodId Parse(string id)
        {
            return new PaymentMethodId(id) { Value = id };
        }
        public static PaymentMethodId Create()
        {
            return new PaymentMethodId(Guid.NewGuid().ToString());
        }
    }
}
