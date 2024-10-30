namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class CartModelPriceDto
    {
        public decimal DefaultPrice { get; set; }
        public decimal DiscountAmountSaved { get; set; } = 0;
        public decimal DiscountPrice { get => DefaultPrice - DiscountAmountSaved; }
        public decimal PromotionAmountSaved { get; set; } = 0;
        public decimal TotalWarrantyPrice { get; set; } = 0;
        public decimal TotalShippingPrice { get; set; } = 0;
        public decimal FinalPrice { get; set; }
    }
}
