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
        public static int MinCode { get; set; } = 4;
        public static int MaxCode { get; set; } = 16;
    }
    public class PromotionRule
    {
        public static PromotionRule Default = new PromotionRule();
        public static string key = "PromotionRule5";
        public static string Type = typeof(PromotionRule).AssemblyQualifiedName;
        public int MaxDiscountPercent { get; set; } = 90;
        public int MinCode { get; set; } = 4;
        public int MaxCode { get; set; } = 16;
        public int MaxRequirement { get; set; } = 4;
        public int MaxGift { get; set; } = 4;
        public decimal MaxOrderDiscount { get; set; } = 50m;
        public decimal MaxOrderReducedAmount { get; set; } = 50_000_000m;
        //public int BronzeUserDiscountPercent { get; set; } = 5;
        //public int SilverUserDiscountPercent { get; set; } = 8;
        //public int GoldUserDiscountPercent { get; set; } = 10;
    }
}
