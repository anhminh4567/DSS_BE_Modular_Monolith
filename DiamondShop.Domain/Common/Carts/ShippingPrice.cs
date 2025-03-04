﻿using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.DeliveryFees;

namespace DiamondShop.Domain.Common.Carts
{
    public class ShippingPrice
    {
        public decimal DefaultPrice { get; set; } = 0;
        public decimal PromotionPrice { get; set; } = 0;
        public decimal UserRankReducedPrice { get; set; } = 0;
        public decimal FinalPrice { get => Math.Clamp(DefaultPrice - PromotionPrice - UserRankReducedPrice,0,decimal.MaxValue); }
        public Address? To { get; set; }
        public Address? From { get; set; }
        public DeliveryFee? DeliveryFeeFounded { get; set; }
        public bool IsValid { get => To != null && From != null
                && (DeliveryFeeFounded != null && DeliveryFeeFounded.Id.Value != DeliveryFee.UNKNONW_DELIVERY_ID)
                && IsLocationActive == true;
        }
        public bool IsLocationActive { get => DeliveryFeeFounded != null && DeliveryFeeFounded.IsEnabled; }
        public ShippingPrice()
        {
            Address? To = null;
            Address? From = null;
            DeliveryFeeFounded = DeliveryFee.CreateUnknownDelivery();
        }
        public static ShippingPrice CreateDeliveryAtShop(Address shopAddress)
        {
            return new ShippingPrice
            {
                DeliveryFeeFounded = DeliveryFee.CreateSelfTakenFromShopDeliveryFee(),
                From = shopAddress,
                To = shopAddress,
            };
        }
    }

}
