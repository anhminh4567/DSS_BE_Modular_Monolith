﻿using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.JewelryModels;

namespace DiamondShop.Domain.Common.Carts
{
    public class CartProduct
    {
        public string CartProductId { get; set; } = DateTime.UtcNow.Ticks.ToString();
        public Jewelry? Jewelry { get; set; }
        public Diamond? Diamond { get; set; }
        public JewelryModel? JewelryModel { get; set; }
        public CheckoutPrice? ReviewPrice { get; set; }
        public string? EngravedText { get; set; }
        public string? EngravedFont { get; set; }
        //public decimal? PurchasedPrice { get; set; }
        public bool IsValid { get; set; }
        public string? DiscountCode { get; set; }
        public int? DiscountPercent { get; set; }
        public string? PromoCode { get; set; }
        public int? PromoPercent { get; set; }
    }

}
