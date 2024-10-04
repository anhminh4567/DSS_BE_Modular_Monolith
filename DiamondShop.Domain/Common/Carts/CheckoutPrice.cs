namespace DiamondShop.Domain.Common.Carts
{
    public class CheckoutPrice
    {
        public decimal DefaultPrice { get; set; } = 0;
        public decimal DiscountAmountSaved { get; set; } = 0;
        //public decimal DiscountPrice { get; set; } = 0;
        public decimal PromotionAmountSaved { get; set; } = 0;
        //public decimal PromotionPrice { get; set; } = 0;
        public decimal FinalPrice { get => DefaultPrice - DiscountAmountSaved - PromotionAmountSaved; }
    }

}
