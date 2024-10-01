using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Jewelries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.Promotions;
using DiamondShop.Domain.Models.Promotions.Entities;

namespace DiamondShop.Domain.Common
{
    public class CartModel
    {
        public CheckoutPrice OrderPrices { get; set; }
        public CartModelCounter OrderCounter { get; set; }
        public CartModelValidation OrderValidation { get; set; }
        public List<CartProduct> Products { get; set; } = new();
    }
    public class CartProduct
    {
        public string CartProductId { get; set; } = DateTime.UtcNow.Ticks.ToString();
        public Jewelry? Jewelry { get; set; }
        public Diamond? Diamond { get; set; }
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
    public class CheckoutPrice
    {
        public decimal DefaultPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal? OrderShippingPrice { get; set; }
    }
    public class CartModelValidation
    {
        public bool IsOrderValid { get; set; }
        public int[] InvalidItemIndex { get; set; } = new int[] { };
    }
    public class CartModelDiscountInfo
    {
        public Promotion? Promotion { get; set; }
        public Discount? Discount { get; set; }
        public decimal TotalPromotionPrice { get; set; }
        public decimal TotalDiscountPrice { get; set; }
    }
    public class CartModelCounter
    {
        public int TotalProduct {  get; set; }
        public int TotalInvalidProduct { get; set; }
        public int TotalItem { get; set; }
        public int TotalInvalidItem { get; set; }
    }
}
