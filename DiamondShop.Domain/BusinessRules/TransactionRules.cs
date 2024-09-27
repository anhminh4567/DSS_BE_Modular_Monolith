using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class TransactionRules
    {
        public long MinimumPerTransaction { get; set; } = 10000;// 10k
        public long MaximumPerTransaction { get; set; } = 200000000;//200 million
        public int TransactionDurationMinute { get; set; } = 15;
    }
}
