using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Domain.Models.Orders.Entities
{
    public class OrderItemWarranty : Entity<OrderItemWarrantyId>
    {
        public OrderItemId OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public WarrantyStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public WarrantyType WarrantyType { get; set; }
        public string WarrantyCode { get; set; }
        public string WarrantyPath { get; set; }
        public decimal SoldPrice { get; set; }
        private OrderItemWarranty() { }
        public static OrderItemWarranty Create(OrderItemId itemId, string warrantyCode, int duration, decimal soldPrice, OrderItemWarrantyId givenId = null)
        {
            return new OrderItemWarranty()
            {
                Id = givenId is null ? OrderItemWarrantyId.Create() : null,
                OrderItemId = itemId,
                WarrantyCode = warrantyCode,
                Status = WarrantyStatus.Active,
                CreatedDate = DateTime.Now,
                EffectiveDate = DateTime.Now,
                ExpiredDate = DateTime.Now.AddMonths(duration),
                SoldPrice = soldPrice
            };
        }
    }
}
