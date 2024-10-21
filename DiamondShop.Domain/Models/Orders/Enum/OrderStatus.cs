using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum OrderStatus
    {
        Pending = 1, Processing = 2, Rejected = 3, Cancelled = 4, Prepared = 5, Delivering = 6, Delivery_Failed = 7, Success = 8, Refused = 9
    }
}
