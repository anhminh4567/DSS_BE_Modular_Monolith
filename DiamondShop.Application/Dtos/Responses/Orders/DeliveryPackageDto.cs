using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Domain.Models.Orders.Enum;

namespace DiamondShop.Application.Dtos.Responses.Orders
{
    public class DeliveryPackageDto
    {
        public string Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public DeliveryPackageStatus Status { get; set; }
        public string? DeliveryMethod { get; set; }
        public string DelivererId { get; set; }
        public AccountDto? Deliverer { get; set; }

    }
}
