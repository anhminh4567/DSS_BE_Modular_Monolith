using DiamondShop.Api.Controllers.Orders.Cancel;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Cancel
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.OrderId).NotEmpty();
            RuleFor(p => p.Reason).NotEmpty();
        }
    }
}
