using FluentValidation;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer
{
    public class CustomerAcceptRequestCommandValidator : AbstractValidator<CustomerAcceptRequestCommand>
    {
        public CustomerAcceptRequestCommandValidator()
        {
            RuleFor(p => p.CustomizeRequestId).NotEmpty();
            RuleFor(p => p.AccountId).NotEmpty();
        }
    }
}
