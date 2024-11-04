using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.Create
{
    public class CreateDiamondPricesCommandValidator : AbstractValidator<CreateDiamondPricesCommand>
    {
        public CreateDiamondPricesCommandValidator()
        {
            RuleFor(c => c.price).NotEmpty().GreaterThan(0);
            RuleFor(c => c.DiamondCriteriaId.Value).NotEmpty();
            RuleFor(c => c.DiamondShapeId.Value).NotEmpty();
        }
    }
}
