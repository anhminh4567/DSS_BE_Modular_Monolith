using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions.Entities;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderRequestDto(string accountId, PaymentType paymentType, string paymentName, bool isTransfer);
}
