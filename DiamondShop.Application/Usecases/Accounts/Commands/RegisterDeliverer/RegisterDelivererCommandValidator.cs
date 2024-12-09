using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RegisterDeliverer
{
    public class RegisterDelivererCommandValidator : AbstractValidator<RegisterDeliveryCommand>
    {
        public RegisterDelivererCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.email).NotEmpty()
                .WithNotEmptyMessage()
                .EmailAddress()
                .WithMessage("không đúng định dạng email");
            RuleFor(x => x.phoneNumber)
          .Cascade(CascadeMode.Stop)
              .MinimumLength(9)
                  .WithMinLenghtMessage()
              .MaximumLength(12)
                  .WithMaxLenghtMessage()
              .Matches(@"^\d+$")
                  .WithMessage("số điện thoại chỉ đươc có số ko kí tự dặc biệt")
              .When(x => x != null);
            RuleFor(x => x.password).NotEmpty().WithNotEmptyMessage();
            RuleFor(x => x.fullName).NotNull().WithMessage("Full name must not be null");
        }
    }
}
