using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public  class DiamondRules
    {
        public static decimal MinPriceOffset { get; set; } = 0.1M;
        public static decimal MaxPriceOffset{ get; set; } = 10M;
    }
}
