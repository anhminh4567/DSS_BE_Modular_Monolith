using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.Jewelries;
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
        public string PromoCode { get; set; }
        public int PromoPercent { get; set; }
        public List<OrderItemDetail> Details { get; set; } 
    }
}
