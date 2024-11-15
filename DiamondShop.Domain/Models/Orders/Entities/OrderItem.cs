using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.Promotions.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Enum;
using DiamondShop.Domain.Models.Promotions.Entities;
using DiamondShop.Domain.Models.Warranties.ValueObjects;
using DiamondShop.Domain.Models.Warranties;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class OrderItem : Entity<OrderItemId>
    {
        public OrderId OrderId { get; set; }
        public OrderItemStatus Status { get; set; }
        public JewelryId? JewelryId { get; set; }
        public Jewelry? Jewelry { get; set; }
        public DiamondId? DiamondId { get; set; }
        public Diamond? Diamond { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal PurchasedPrice { get; set; }
        public DiscountId? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public int? DiscountPercent { get; set; }
        public UnitType? PromoType { get; set; }
        public decimal? PromoValue { get; set; }
        public decimal WarrantyPrice { get; set; }
        //public List<OrderItemWarranty>? Warranties { get; set; } = new();
        public OrderItemWarrantyId? WarrantyId { get; set; }
        public OrderItemWarranty? Warranty { get; set; }
        public OrderItem() { }
        public static OrderItem Create(OrderId orderId, JewelryId? jewelryId, DiamondId? diamondId,decimal? originalPrice, decimal? purchasedPrice = 0, DiscountId? discountId = null,  int? discountPercent = 0, UnitType? promoType = UnitType.Percent, decimal? promoValue = 0, decimal warrantyPrice = 0, OrderItemId? givenId = null)
        {
            return new OrderItem()
            {
                Id = givenId is null ? OrderItemId.Create() : givenId,
                OrderId = orderId,
                Status = OrderItemStatus.Pending,
                JewelryId = jewelryId,
                DiamondId = diamondId,
                //FinalPrice = finalPrice ?? 0,
                OriginalPrice = originalPrice ?? 0,
                PurchasedPrice = purchasedPrice ?? 0,
                DiscountId = discountId,
                DiscountPercent = discountPercent ?? 0,
                PromoType = promoType,
                PromoValue = promoValue ?? 0,
                WarrantyPrice = warrantyPrice
            };
        }
    }
}
