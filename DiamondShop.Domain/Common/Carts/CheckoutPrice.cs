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
        public decimal PromotionPrice { get => DefaultPrice - DiscountAmountSaved - PromotionAmountSaved; }
        //warranty is not part of any discount
        //warranty is seperate from final price
        //warranty is not subject of any promotion requirement or condition
        public decimal WarrantyPrice { get; set; } = 0;
        public decimal FinalPrice { get => Math.Clamp(
                MoneyVndRoundUpRules.RoundAmountFromDecimal(DefaultPrice - DiscountAmountSaved - PromotionAmountSaved),0,decimal.MaxValue) ; }
    }

}
