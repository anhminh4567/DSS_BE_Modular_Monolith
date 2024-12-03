using DiamondShop.Domain.Models.AccountAggregate.Entities;
using DiamondShop.Domain.Models.Orders.Enum;
using DiamondShop.Domain.Models.Transactions.Entities;

namespace DiamondShop.Application.Dtos.Requests.Orders
{
    public record OrderRequestDto(PaymentType PaymentType,string PaymentId, string PaymentName, string? PromotionId, bool IsTransfer, bool? IsAtShop = false);
    public record BillingDetail(string FirstName, string LastName, string Phone, string Email, string Providence, string District, string Ward, string Address, string? Note)
    {
        public string GetAddressString()
        {
             string address = String.Join(" ", [Providence, District, Ward, Address]);
            return address;
        }
    }
}
