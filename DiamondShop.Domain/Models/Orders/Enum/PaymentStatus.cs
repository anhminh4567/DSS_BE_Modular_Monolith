using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum  PaymentStatus
    {
        Pending = 1, Deposited = 2, Paid = 3, Refunding = 4, Refunded = 5 , No_Refunded = 6
    }
}
