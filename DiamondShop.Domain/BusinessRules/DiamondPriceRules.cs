using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class DiamondPriceRules
    {
        public decimal MinPriceOffsetPercent { get; set; } = 100 - 90;//%
        public decimal MaxPriceOffsetPercent { get; set; } = 100 + 90;//%
    }
}
