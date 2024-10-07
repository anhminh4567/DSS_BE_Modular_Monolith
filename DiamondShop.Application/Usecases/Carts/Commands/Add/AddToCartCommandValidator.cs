using FluentValidation;

namespace DiamondShop.Application.Usecases.Carts.Commands.Add
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
            RuleFor(x => x.Jewelry).NotNull()
                .When(x => x.Diamond == null && x.JewelryModel == null)
                .WithMessage("can only add 1 in 3 , diamond, jewelry, jewelry model, if one is choosen, the order must be NULL");
            RuleFor(x => x.Diamond).NotNull()
                .When(x => x.Jewelry == null && x.JewelryModel == null)
                .WithMessage("can only add 1 in 3 , diamond, jewelry, jewelry model, if one is choosen, the order must be NULL");
            RuleFor(x => x.JewelryModel).NotNull()
                .When(x => x.Jewelry == null && x.Diamond == null)
                .WithMessage("can only add 1 in 3 , diamond, jewelry, jewelry model, if one is choosen, the order must be NULL");

        }
    }
}
