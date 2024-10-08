namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class ShippingPriceDto
    {
        public decimal DefaultPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
