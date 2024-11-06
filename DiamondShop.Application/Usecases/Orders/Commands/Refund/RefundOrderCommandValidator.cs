using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Refund
{
    public class RefundOrderCommandValidator : AbstractValidator<RefundOrderCommand>
    {
        public RefundOrderCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty();
        }
    }
}
