using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class OrderPaymentRules
    {
        public static int DepositPercent { get; set; } = 50;
        public static int CODPercent { get; set; } = 10;
        public static int CODPercentCustom { get; set; } = 50;
        public static int PayAllFine { get; set; } = 10;
        public static decimal MaxMoneyFine { get; set; } = 5_000_000m;
        public static decimal MinAmountForCOD { get; set; } = 100_000m;
    }
}
