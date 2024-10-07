using FluentValidation;

namespace DiamondShop.Application.Usecases.Carts.Commands.Update
{
    public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
    {
        public UpdateCartItemCommandValidator()
        {
            RuleFor(x => x.accountId).NotEmpty();
            RuleFor(x => x.cartItemIdToRemove).NotEmpty();
            RuleFor(x => x.AddCommand).NotNull();
        }
    }
}
