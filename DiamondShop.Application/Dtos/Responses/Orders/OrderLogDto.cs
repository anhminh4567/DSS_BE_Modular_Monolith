using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.Orders.Entities;
using DiamondShop.Domain.Models.Orders.ValueObjects;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class OrderLogDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? PreviousLogId { get; set; }
        public OrderLogDto? PreviousLog { get; set; }

        public List<Media>? LogImages { get; set; }
        public string? DeliveryPackageId { get; set; }

    }
}
