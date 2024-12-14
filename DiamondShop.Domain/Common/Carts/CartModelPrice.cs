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
        public decimal OrderPriceExcludeShipAndWarranty { get => DefaultPrice - DiscountAmountSaved - ProductPromotionAmountSaved; }
        public decimal OrderAmountSaved { get; set; } = 0;//when user reach a rank or promotion order
        // shipping and warranty may sit outside discount and promotion scope
        public decimal PromotionAmountSaved { get => OrderAmountSaved + ProductPromotionAmountSaved; }
        public decimal TotalWarrantyPrice { get; set; } = 0;
        public decimal TotalShippingPrice { get; set; } = 0;
        public decimal FinalPrice { get => Math.Clamp(
                MoneyVndRoundUpRules
                .RoundAmountFromDecimal(DefaultPrice - DiscountAmountSaved - ProductPromotionAmountSaved - OrderAmountSaved - UserRankDiscountAmount + TotalShippingPrice + TotalWarrantyPrice)
            ,0m, decimal.MaxValue); }
        public bool IsFreeOrder { get => FinalPrice == 0; }
        
        
    }

}
