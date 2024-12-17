using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Dtos.Responses.Deliveries;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DeliveryFees;

namespace DiamondShop.Application.Dtos.Responses.Carts
{
    public class ShippingPriceDto
    {
        public decimal DefaultPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal UserRankReducedPrice { get; set; } 
        public decimal FinalPrice { get; set; }
        public AddressDto? To { get; set; }
        public AddressDto? From { get; set; }
        public DeliveryFeeDto? DeliveryFeeFounded { get; set; }
        public bool IsValid { get; set; }
        public bool IsSameCityDelivery { get => From?.Province == To?.Province; }
        public bool IsLocationActive { get; set; }
    }
}
