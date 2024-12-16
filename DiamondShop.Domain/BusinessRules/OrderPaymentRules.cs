using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class OrderPaymentRules
    {
        public static OrderPaymentRules Default = new OrderPaymentRules();
        public static string Type = typeof(OrderPaymentRules).AssemblyQualifiedName;
        public static string key = "OrderPaymentRuleVer3";
        public int DepositPercent { get; set; } = 50;
        public int CODPercent { get; set; } = 10;
        //public int CODPercentCustom { get; set; } = 50;
        public int PayAllFine { get; set; } = 10;
        public decimal MaxMoneyFine { get; set; } = 5_000_000m;
        public decimal MinAmountForCOD { get; set; } = 100_000m;
        public int CODHourTimeLimit { get; set; } = 4;
        public List<string> LockedPaymentMethodOnCustomer { get; set; } = new List<string> { "3" };
        public List<string> LockedPaymentMethodOnShop { get; set; } = new List<string> { "2" };
    }
}
