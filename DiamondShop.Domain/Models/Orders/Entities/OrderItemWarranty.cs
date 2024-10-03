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
    public class OrderItemWarranty 
    {
        public OrderItemId OrderItemId { get; set; }
        public string ItemId { get; set; }
        public WarrantyStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public WarrantyType WarrantyType { get; set; }
        public string WarrantyCode { get; set; }
        public string WarrantyPath { get; set; }
        public decimal SoldPrice { get; set; }
        public OrderItemWarranty() { }
    }
}
