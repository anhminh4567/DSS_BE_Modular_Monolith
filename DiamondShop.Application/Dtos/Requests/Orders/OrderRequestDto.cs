using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions.Entities;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderRequestDto(PaymentType PaymentType,string PaymentId, string PaymentName, string? PromotionId, bool IsTransfer);
    public record BillingDetail(string FirstName, string LastName, string Phone, string Email, string Providence, string District, string Ward, string Address, string? Note);
}
