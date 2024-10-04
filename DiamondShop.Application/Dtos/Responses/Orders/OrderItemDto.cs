using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Dtos.Responses.Jewelries;
using DiamondShop.Domain.Models.Diamonds.ValueObjects;
using DiamondShop.Domain.Models.Jewelries.ValueObjects;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderItemDto
    {
        public string Id { get; set; }
        public OrderId OrderId { get; set; }
        public OrderItemStatus Status { get; set; }
        public JewelryId? JewelryId { get; set; }
        public JewelryDto? Jewelry { get; set; }
        public DiamondId? DiamondId { get; set; }
        public DiamondDto? Diamond { get; set; }
        public string EngravedText { get; set; }
        public string EngravedFont { get; set; }
        public decimal PurchasedPrice { get; set; }
        public string DiscountCode { get; set; }
        public int DiscountPercent { get; set; }
        public string PromoCode { get; set; }
        public int PromoPercent { get; set; }
        public List<OrderItemWarrantyDto>? Warranties { get; set; } = new();

    }
}
