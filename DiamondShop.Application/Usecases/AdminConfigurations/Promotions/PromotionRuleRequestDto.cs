using DiamondShop.Domain.Models.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions
{
    public class PromotionRuleRequestDto
    {
        public int? MaxDiscountPercent { get; set; }
        public int? MinCode { get; set; }
        public int? MaxCode { get; set; }
        public int? MaxRequirement { get; set; } 
        public int? MaxGift { get; set; } 
        public decimal? MaxOrderDiscount { get; set; } 
        public decimal? MaxOrderReducedAmount { get; set; }
        //public int? BronzeUserDiscountPercent { get; set; }
        //public int? SilverUserDiscountPercent { get; set; }
        //public int? GoldUserDiscountPercent { get; set; }
    }
}
