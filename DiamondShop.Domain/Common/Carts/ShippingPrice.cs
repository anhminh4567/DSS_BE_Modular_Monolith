using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DeliveryFees;

namespace DiamondShop.Domain.Common.Carts
{
    public class ShippingPrice
    {
        public decimal DefaultPrice { get; set; } = 0;
        public decimal PromotionPrice { get; set; } = 0;
        public decimal FinalPrice { get; set; } = 0;
        public Address? To { get; set; }
        public Address? From { get; set; }
        public DeliveryFee? DeliveryFeeFounded { get; set; }
        public bool IsValid { get => To != null && From != null && ( DeliveryFeeFounded !=null && DeliveryFeeFounded.Id.Value == DeliveryFee.UNKNONW_DELIVERY_ID); }
        public ShippingPrice()
        {
            Address? To = null;
            Address? From = null;
            DeliveryFeeFounded = DeliveryFee.CreateUnknownDelivery();
        }
    }

}
