using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Diamonds;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class OrderItemDetail : Entity<OrderItemDetailId>
    {
        public OrderItemId ItemId { get; set; }
        public JewelryId? JewelryId { get; set; }
        public Jewelry? Jewelry { get; set; }
        public DiamondId? DiamondId { get; set; }
        public Diamond? Diamond { get; set; }
        public bool IsGifted { get; set; }
        public string EngravedText { get; set; }
        public string EngravedFont { get; set; }
        public decimal PurchasedPrice { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public OrderItemDetailId? MainDetailId { get; set; }
        public OrderItemDetail? MainDetail { get; set; }
    }
}
