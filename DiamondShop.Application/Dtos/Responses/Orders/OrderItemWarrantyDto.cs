using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Warranties.Enum;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderItemWarrantyDto
    {
        public string OrderItemId { get; set; }
        public string ItemId { get; set; }
        public WarrantyStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public WarrantyType WarrantyType { get; set; }
        public string WarrantyCode { get; set; }
        public string WarrantyPath { get; set; }
        public decimal SoldPrice { get; set; }

    }
}
