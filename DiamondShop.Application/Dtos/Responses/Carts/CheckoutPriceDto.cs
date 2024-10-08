namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CheckoutPriceDto
    {
        // properties of CheckoutPriceDto
        public decimal DefaultPrice { get; set; }
        public decimal DiscountAmountSaved { get; set; } 
        public decimal DiscountPrice { get; set; }
        public decimal PromotionAmountSaved { get; set; }
        //public decimal PromotionPrice { get; set; } = 0;
        public decimal FinalPrice { get; set; }
    }
}
