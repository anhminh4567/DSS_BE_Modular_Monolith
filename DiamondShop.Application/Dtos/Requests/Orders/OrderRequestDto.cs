using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions.Entities;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderRequestDto(string accountId, decimal totalPrice, PaymentMethod paymentMethod, PaymentType paymentType);
}
