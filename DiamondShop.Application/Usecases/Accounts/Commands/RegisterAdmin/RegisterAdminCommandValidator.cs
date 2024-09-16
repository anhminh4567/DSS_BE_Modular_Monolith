using FluentValidation;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterAdmin
{
    public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
    {
        public RegisterAdminCommandValidator()
        {
            RuleFor(c => c.email).NotEmpty().EmailAddress();
            RuleFor(c => c.password).NotEmpty();

            RuleFor(c => c.fullName.FirstName).NotEmpty();
            RuleFor(c => c.fullName.LastName).NotEmpty();
            RuleFor(c => c.fullName).NotNull();
        }
    }
}
