using FluentValidation;

namespace DiamondShop.Application.Usecases.Staffs.Commands.Security.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(c => c.email).EmailAddress().NotEmpty();
            RuleFor(c => c.password).NotEmpty();
        }
    }
}
