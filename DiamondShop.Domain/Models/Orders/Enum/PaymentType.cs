using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum PaymentType
    {
        // business rules, if COD for order custom then 50%, else 10%
         Payall = 1, COD = 2
    }
}
