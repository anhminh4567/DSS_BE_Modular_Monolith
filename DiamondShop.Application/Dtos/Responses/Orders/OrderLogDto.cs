using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderLogDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
        public string CreatedDate { get; set; }
        public OrderStatus Status { get; set; }
        public string? PreviousLogId { get; set; }
        public OrderLogDto? PreviousLog { get; set; }

        public List<MediaDto>? LogImages { get; set; }
        public string? DeliveryPackageId { get; set; }
        public List<OrderLogDto> ChildLog { get; set; } = new();
        public bool IsParentLog { get; set; }
    }
}
