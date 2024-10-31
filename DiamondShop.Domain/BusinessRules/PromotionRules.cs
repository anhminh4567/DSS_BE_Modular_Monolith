using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public static class PromotionRules
    {
        public static int MaxDiscountPercent { get; set; } = 90;
        public static int MinCode { get; set; } = 10;
        public static int MaxCode { get; set; } = 16;
    }
    public class PromotionRule
    {
        public static string Type = typeof(PromotionRule).Name;
        public int MaxDiscountPercent { get; set; } = 90;
        public int MinCode { get; set; } = 10;
        public int MaxCode { get; set; } = 16;
    }
}
