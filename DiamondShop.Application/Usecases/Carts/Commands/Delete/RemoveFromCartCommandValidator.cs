using FluentValidation;

namespace DiamondShop.Application.Usecases.Carts.Commands.Delete
{
    public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
    {
        public RemoveFromCartCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
            RuleFor(x => x.cartItemId).NotEmpty();
        }
    }   
}
