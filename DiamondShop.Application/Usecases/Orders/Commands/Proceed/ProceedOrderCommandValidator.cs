using DiamondShop.Application.Usecases.Orders.Commands.Proceed;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.AssignDeliverer
{
    public class ProceedOrderCommandValidator : AbstractValidator<ProceedOrderCommand>
    {
        public ProceedOrderCommandValidator()
        {
            RuleFor(p => p.orderId).NotEmpty();
            RuleFor(p => p.accountId).NotEmpty();
        }
    }
}
