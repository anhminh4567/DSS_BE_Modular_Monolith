using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions.Entities;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderRequestDto(PaymentType PaymentType, string PaymentName, bool IsTransfer);
}
