using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.Orders;
using DiamondShop.Domain.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        public AccountServices()
        {
        }
        public decimal GetUserPointFromOrderComplete(Account userAccount, Order completedOrder)
        {
            return GetUserPointFromOrderCompleteGlobal(userAccount, completedOrder);
        }
        public static decimal GetUserPointFromOrderCompleteGlobal(Account userAccount, Order completedOrder)
        {
            var orderTotal = completedOrder.TotalPrice;
            var convertedToPoint = AccountRules.OrderPriceToPoint(orderTotal);
            return convertedToPoint;
        }
    }
}
