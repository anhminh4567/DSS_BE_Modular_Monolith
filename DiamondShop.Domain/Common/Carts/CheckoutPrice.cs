namespace DiamondShop.Domain.Common.Carts
{
    public class CheckoutPrice
    {
        public decimal DefaultPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }

}
