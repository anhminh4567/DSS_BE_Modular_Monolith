using DiamondShop.Application.Dtos.Requests.Carts;
using FluentValidation;
using DiamondShop.Application.Dtos.Requests.Accounts;

namespace DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson
{
    public class ValidateCartFromListCommandValidator : AbstractValidator<ValidateCartFromListCommand>
    {
        public ValidateCartFromListCommandValidator()
        {
            var cartItemRequestDtoValidator = new CartItemRequestDtoValidator();
            RuleFor(x => x.items).NotNull();
            When(x => x.items.UserAddress != null, () => 
            {
                RuleFor( x => x.items.UserAddress!).SetValidator(new AddressRequestDtoValidator());
            });
            RuleForEach( x => x.items.Items).SetValidator(cartItemRequestDtoValidator);
        }
    }

}
