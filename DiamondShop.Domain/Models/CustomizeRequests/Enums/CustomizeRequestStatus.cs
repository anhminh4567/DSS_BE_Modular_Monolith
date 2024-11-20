using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.CustomizeRequests.Enums
{
    public enum CustomizeRequestStatus
    {
         Pending = 1, Priced = 2, Requesting = 3, Accepted = 4, Shop_Rejected = 5, Customer_Rejected = 6, Customer_Cancelled = 7
    }
}
