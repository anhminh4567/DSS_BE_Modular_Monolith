using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Reject
{
    public class RejectOrderCommandValidator : AbstractValidator<RejectOrderCommand>
    {
        public RejectOrderCommandValidator()
        {
            RuleFor(p => p.OrderId).NotEmpty();
            RuleFor(p => p.UserId).NotEmpty();
            RuleFor(p => p.Reason).NotEmpty();
        }
    }
}
