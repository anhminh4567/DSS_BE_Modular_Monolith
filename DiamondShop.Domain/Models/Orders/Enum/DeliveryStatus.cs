using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Enum
{
    public enum DeliveryStatus
    {
        Waiting = 1, Ready = 2, Delivering = 3, Success = 4, Failed = 5, Abort = 6 
    }
}
