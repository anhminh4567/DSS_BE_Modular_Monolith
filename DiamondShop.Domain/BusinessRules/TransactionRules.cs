using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public static class TransactionRules
    {
        public static long MinimumPerTransaction { get; set; } = 10000;// 10k
        public static long MaximumPerTransaction { get; set; } = 200000000;//200 million
        public static int TransactionDurationMinute { get; set; } = 15;
    }
}
