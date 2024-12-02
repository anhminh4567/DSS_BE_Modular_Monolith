using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.UpdateMany
{
    public class UpdateManyDiamondPricesCommandValidator : AbstractValidator<UpdateManyDiamondPricesCommand>
    {
        public UpdateManyDiamondPricesCommandValidator()
        {
            var validator = new UpdatedDiamondPriceValidator();
            //RuleFor(x => x.isFancyShapePrice).NotNull();
            RuleForEach(x => x.updatedDiamondPrices)
                .SetValidator(validator);
        }
        private class UpdatedDiamondPriceValidator : AbstractValidator<UpdatedDiamondPrice>
        {
            public UpdatedDiamondPriceValidator()
            {
                RuleFor(x => x.diamondPriceId).NotEmpty()
                    .WithNotEmptyMessage();
                RuleFor(x => x.price).NotNull()
                        .WithNotEmptyMessage()
                    .GreaterThan(0)
                        .WithGreaterThanMessage();
            }
    }
}
}
