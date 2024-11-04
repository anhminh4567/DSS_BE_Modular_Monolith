using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class AccountRules
    {
        public static AccountRules Default = new AccountRules();
        public static string key = "AccountRulesV2";
        public static string Type = typeof(AccountRules).AssemblyQualifiedName;
        public int MaxAddress { get; set; } = 5;
        public decimal VndPerPoint { get; set; } = 100_000m;
        public decimal TotalPointToBronze { get; set; } = 100;
        public decimal TotalPointToSilver { get; set; } = 1000;
        public decimal TotalPointToGold { get; set; } = 5000;
        public static decimal OrderPriceToPoint(decimal orderTotalPrice)
        {
            if(orderTotalPrice <= 0) 
                return 0;
            decimal result = orderTotalPrice / Default.VndPerPoint;
            return Math.Floor(result);
        }
    }
}
