using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum OrderStatus
    {
        Pending, Accepted, Cancelled, Processing, Rejected, Finished, Shipping, Ship_Cancelled, Ship_Failed, Ship_Returning, Ship_Returned, Received
    }
}
