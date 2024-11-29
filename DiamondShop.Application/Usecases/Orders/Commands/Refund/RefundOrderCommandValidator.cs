using FluentValidation;

namespace DiamondShop.Application.Usecases.Orders.Commands.Refund
{
    public class RefundOrderCommandValidator : AbstractValidator<RefundOrderCommand>
    {
        public RefundOrderCommandValidator()
        {
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.RefundConfirmRequestDto).ChildRules(p =>
            {
                p.RuleFor(k => k.OrderId).NotEmpty();
                p.RuleFor(k => k.TransactionCode).NotEmpty();
                p.RuleFor(k => k.Amount).NotEmpty();
            });
        }
    }
}
