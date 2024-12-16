using DiamondShop.Domain.BusinessRules;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartModelPrice
    {
        public decimal UserRankDiscountPercent { get; set; } = 0;
        public decimal UserRankDiscountAmount { get; set; } = 0;
        //
        private decimal _defaultPrice;
        public decimal DefaultPrice
        {
            get => _defaultPrice;
            set => _defaultPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(value);
        }
        public decimal DiscountAmountSaved { get; set; } = 0;
        public decimal DiscountPrice { get => DefaultPrice - DiscountAmountSaved; }
        public decimal ProductPromotionAmountSaved { get; set; } = 0;
        public decimal OrderPriceExcludeShip { get => 
                 Math.Clamp(DefaultPrice - DiscountAmountSaved - ProductPromotionAmountSaved  + TotalWarrantyPrice, 0,decimal.MaxValue);
        }//+ TotalShippingPrice
        public decimal OrderAmountSaved { get; set; } = 0;//when user reach a rank or promotion order
        // shipping and warranty may sit outside discount and promotion scope
        public decimal PromotionAmountSaved { get => OrderAmountSaved + ProductPromotionAmountSaved; }
        public decimal TotalWarrantyPrice { get; set; } = 0;
        public decimal ShippingPriceSaved { get; set; } = 0;
        public decimal TotalShippingPrice { get; set; } = 0;
        public decimal FinalShippingPrice { get => TotalShippingPrice - ShippingPriceSaved; }
        public decimal FinalPriceBeforeShippingAndUserRank 
        {
            get => Math.Clamp(
                MoneyVndRoundUpRules
                .RoundAmountFromDecimal(OrderPriceExcludeShip - OrderAmountSaved  )//- UserRankDiscountAmount
            , 0m, decimal.MaxValue);
        }//+ TotalShippingPrice
        public decimal FinalPrice { get => Math.Clamp(
                MoneyVndRoundUpRules
                .RoundAmountFromDecimal(FinalPriceBeforeShippingAndUserRank + FinalShippingPrice - UserRankDiscountAmount)
            ,0m, decimal.MaxValue);
        }//+ TotalShippingPrice
        public bool IsFreeOrder { get => FinalPrice == 0; }
        
        
    }

}
