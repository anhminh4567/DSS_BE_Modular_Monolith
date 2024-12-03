using DiamondShop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.BusinessRules
{
    public class AccountRules
    {
        public static AccountRules Default = new AccountRules();
        public static string key = "AccountRulesV4";
        public static string Type = typeof(AccountRules).AssemblyQualifiedName;
        public int MaxAddress { get; set; } = 5;
        public decimal VndPerPoint { get; set; } = 100_000m;
        public decimal TotalPointToBronze { get; set; } = 100;
        public decimal TotalPointToSilver { get; set; } = 1000;
        public decimal TotalPointToGold { get; set; } = 5000;
        public RankingBenefit GoldRankBenefit { get; set; } = new RankingBenefit()
        {
            RankDiscountPercentOnOrder = 10,
            MaxAmountDiscountOnOrder = 10_000_000_000m,//10 ty limit, coi nhu unlimit
            RankDiscountPercentOnShipping = 100,
        };
        public RankingBenefit SilverRankBenefit { get; set; } = new RankingBenefit()
        {
            RankDiscountPercentOnOrder = 5,
            MaxAmountDiscountOnOrder = 50_000_000m,//5 ty limit, coi nhu unlimit
            RankDiscountPercentOnShipping = 50,
        };
        public RankingBenefit BronzeRankBenefit { get; set; } = new RankingBenefit()
        {
            RankDiscountPercentOnOrder = 3,
            MaxAmountDiscountOnOrder = 20_000_000m,//1 ty limit, coi nhu unlimit
            RankDiscountPercentOnShipping = 10,
        };
        public static decimal OrderPriceToPoint(decimal orderTotalPrice)
        {
            if(orderTotalPrice <= 0) 
                return 0;
            decimal result = orderTotalPrice / Default.VndPerPoint;
            return Math.Floor(result);
        }
    }
    public class RankingBenefit
    {
        public int RankDiscountPercentOnOrder { get; set; } = 5;
        public decimal MaxAmountDiscountOnOrder { get; set; } = 1_000_000m;
        public int RankDiscountPercentOnShipping { get; set; } = 10;
        //public int BronzeUserDiscountPercent { get; set; } = 5;
        //public int SilverUserDiscountPercent { get; set; } = 8;
        //public int GoldUserDiscountPercent { get; set; } = 10;
    }
}
