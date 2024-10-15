using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum OrderStatus
    {
        Pending, Processing, Rejected, Cancelled, Prepared, Delivering, Delivery_Failed, Success, Refused
    }
}
