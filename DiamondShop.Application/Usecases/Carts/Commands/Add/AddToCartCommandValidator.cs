using FluentValidation;

namespace DiamondShop.Application.Usecases.Carts.Commands.Add
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty();
            RuleFor(x => x.Jewelry).NotNull().When(x => x.Diamond == null && x.JewelryModel == null);
            RuleFor(x => x.Diamond).NotNull().When(x => x.Jewelry == null && x.JewelryModel == null);
            RuleFor(x => x.JewelryModel).NotNull().When(x => x.Jewelry == null && x.Diamond == null);

        }
    }
}
