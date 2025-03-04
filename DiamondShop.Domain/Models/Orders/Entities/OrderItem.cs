﻿using DiamondShop.Domain.Common;
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
using System.ComponentModel.DataAnnotations.Schema;
using DiamondShop.Domain.Models.Jewelries.Entities;

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
        public string? DiscountCode { get; set; }
        public decimal? DiscountSavedAmount { get; set; }
        //public int? DiscountPercent { get; set; }
        //public UnitType? PromoType { get; set; }
        //public decimal? PromoValue { get; set; }
        public decimal? PromotionSavedAmount { get; set; }
        public decimal WarrantyPrice { get; set; }

        //public List<OrderItemWarranty>? Warranties { get; set; } = new();
        public OrderItemWarrantyId? WarrantyId { get; set; }
        public OrderItemWarranty? Warranty { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public bool IsProductDelete { get{ return (JewelryId == null && DiamondId == null);  } }
        public OrderItem() { }
        public static OrderItem Create(OrderId orderId, string name, JewelryId? jewelryId, DiamondId? diamondId,decimal? originalPrice, decimal? purchasedPrice = 0, Discount? discount = null, decimal? promotionAmountSaved = null, decimal? discountAmountSaved = null, decimal warrantyPrice = 0, OrderItemId? givenId = null)
        {
            var productItem = jewelryId is null ? diamondId.Value : jewelryId.Value;
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
                DiscountId = discount == null ? null : discount.Id ,
                DiscountCode = discount == null ? null : discount.DiscountCode,
                PromotionSavedAmount = promotionAmountSaved,
                DiscountSavedAmount = discountAmountSaved,
                WarrantyPrice = warrantyPrice,
                ProductId = productItem,
                Name = name,
            };
        }
        public void SetCancel()
        {
            JewelryId = null;
            DiamondId = null;
            Status = OrderItemStatus.Removed;
        }
    }
}
