using DiamondShop.Domain.Models.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Dtos.Requests.ApplicationConfigurations.Promotions
{
    public class PromotionRuleRequestDto
    {
        public int? MaxDiscountPercent { get; set; } 
        public int? MinCode { get; set; } 
        public int? MaxCode { get; set; } 
        public int? BronzeUserDiscountPercent { get; set; } 
        public int? SilverUserDiscountPercent { get; set; } 
        public int? GoldUserDiscountPercent { get; set; } 
    }
}
