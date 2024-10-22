using DiamondShop.Domain.BusinessRules;

namespace DiamondShop.Domain.Common.Carts
{
    public class CheckoutPrice
    {
        private decimal _defaultPrice ;
        public decimal DefaultPrice { 
            get => _defaultPrice; 
            set => _defaultPrice = MoneyVndRoundUpRules.RoundAmountFromDecimal(value); 
        }
        public decimal DiscountAmountSaved { get; set; } = 0;
        public decimal DiscountPrice { get => DefaultPrice - DiscountAmountSaved ; }
        public decimal PromotionAmountSaved { get; set; } = 0;
        //public decimal PromotionPrice { get; set; } = 0;
        public decimal FinalPrice { get => MoneyVndRoundUpRules.RoundAmountFromDecimal(DefaultPrice - DiscountAmountSaved - PromotionAmountSaved) ; }
    }

}
