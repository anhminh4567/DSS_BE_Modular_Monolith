using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.UpdateMany
{
    public class UpdateManyDiamondPricesCommandValidator : AbstractValidator<UpdateManyDiamondPricesCommand>
    {
        public UpdateManyDiamondPricesCommandValidator()
        {
            var validator = new UpdatedDiamondPriceValidator();
            RuleFor(x => x.isFancyShapePrice).NotNull();
            RuleForEach(x => x.updatedDiamondPrices)
                .SetValidator(validator);
        }
        private class UpdatedDiamondPriceValidator : AbstractValidator<UpdatedDiamondPrice>
        {
            public UpdatedDiamondPriceValidator()
            {
                RuleFor(x => x.diamondCriteriaId).NotEmpty();
                RuleFor(x => x.price).NotNull().GreaterThan(0);
            }
    }
}
}
