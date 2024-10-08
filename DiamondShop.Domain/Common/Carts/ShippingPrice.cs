using DiamondShop.Domain.Models.AccountAggregate.Entities;

namespace DiamondShop.Domain.Common.Carts
{
    public class ShippingPrice
    {
        public decimal DefaultPrice { get; set; } = 0;
        public decimal PromotionPrice { get; set; } = 0;
        public decimal FinalPrice { get; set; } = 0;
        public Address? To { get; set; }
        public Address? From { get; set; }
    }

}
