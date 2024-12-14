using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;
using DiamondShop.Domain.Models.Promotions.Enum;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderItemDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public OrderItemStatus Status { get; set; }
        public string? JewelryId { get; set; }
        public JewelryDto? Jewelry { get; set; }
        public string? DiamondId { get; set; }
        public DiamondDto? Diamond { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal PurchasedPrice { get; set; }
        public string? DiscountCode { get; set; }
        public string? WarrantyId { get; set; }
        public decimal? DiscountSavedAmount { get; set; }
        public decimal? PromotionSavedAmount { get; set; }
        public decimal WarrantyPrice { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public bool IsProductDelete { get; set; }
        public List<OrderItemWarrantyDto>? Warranties { get; set; } = new();

    }
}
