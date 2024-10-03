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
        public string EngravedText { get; set; }
        public string EngravedFont { get; set; }
        public decimal PurchasedPrice { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public string PromoCode { get; set; }
        public int PromoPercent { get; set; }
        public List<OrderItemWarranty>? Warranties { get; set; } = new();
        public OrderItem() { }
    }
}
