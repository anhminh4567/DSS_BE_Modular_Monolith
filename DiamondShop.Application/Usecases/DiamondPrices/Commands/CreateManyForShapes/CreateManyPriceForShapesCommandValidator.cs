using FluentValidation;

namespace DiamondShop.Application.Usecases.DiamondPrices.Commands.CreateManyForShapes
{
    public class CreateManyPriceForShapesCommandValidator : AbstractValidator<CreateManyPriceForShapesCommand>
    {
        public CreateManyPriceForShapesCommandValidator()
        {
            RuleFor(x => x.criteriaId).NotEmpty();
            RuleFor(x => x.pricesForShape).NotNull();
            RuleForEach(x => x.pricesForShape).SetValidator(new PriceForShapeValidator());
            RuleFor(x => x.pricesForShape)// check if the shapesId is distinct, to avoid error
                .Must(x => x.Select(c => c.shapeId).Distinct().Count() == x.Count());

        }
        public class PriceForShapeValidator : AbstractValidator<PriceForShape>
        {
            public PriceForShapeValidator()
            {
                RuleFor(x => x.shapeId).NotEmpty();
                RuleFor(x => x.price).GreaterThan(0);
            }
        }
    }
}
