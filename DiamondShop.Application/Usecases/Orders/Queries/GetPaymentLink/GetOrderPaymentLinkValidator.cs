using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Queries.GetPaymentLink
{
    public class GetOrderPaymentLinkValidator : AbstractValidator<GetOrderPaymentLink>
    {
        public GetOrderPaymentLinkValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty();
        }
    }
}
