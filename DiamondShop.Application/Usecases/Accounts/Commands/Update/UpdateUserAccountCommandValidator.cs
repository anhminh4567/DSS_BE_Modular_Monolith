using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Application.Dtos.Requests.Accounts;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Accounts.Commands.Update
{
    public class UpdateUserAccountCommandValidator : AbstractValidator<UpdateUserAccountCommand>
    {
        public UpdateUserAccountCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
            When(x => x.ChangedFullName != null, () =>
            {
                RuleFor(x => x.ChangedFullName.FirstName).NotEmpty().WithMessage("First name must not be empty");
                RuleFor(x => x.ChangedFullName.LastName).NotEmpty().WithMessage("Last name must not be empty");
            });
            When(x => x.ChangedAddress != null, () =>
            {
                When(xx => xx.ChangedAddress!.updatedAddress != null, () =>
                {
                    RuleForEach(x => x.ChangedAddress!.updatedAddress).ChildRules(address =>
                    {
                        address.RuleFor(x => x.Value).SetValidator(new AddressRequestDtoValidator());
                    });
                });
                When(xx => xx.ChangedAddress!.addedAddress != null, () =>
                {
                    RuleForEach(x => x.ChangedAddress!.addedAddress).SetValidator(new AddressRequestDtoValidator());
                });
            });
            RuleFor(x => x.newPhoneNumber)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(9)
                    .WithMinLenghtMessage()
                .MaximumLength(12)
                    .WithMaxLenghtMessage()
                .Matches(@"^\d+$")
                    .WithMessage("số điện thoại chỉ đươc có số ko kí tự dặc biệt")
                .When(x => x != null);
        }
    }

}
