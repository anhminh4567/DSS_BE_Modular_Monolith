using FluentValidation;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.Create
{
    public class CreateDiamondCommandValidator : AbstractValidator<CreateDiamondCommand>
    {
        public CreateDiamondCommandValidator()
        {
            RuleFor(c => c.shapeId).NotEmpty();
            RuleFor(c => c.diamond4c.Cut).NotEmpty();
            RuleFor(c => c.diamond4c.Color).NotEmpty();
            RuleFor(c => c.diamond4c.Clarity).NotEmpty();
            RuleFor(c => c.diamond4c.Carat).NotEmpty();
            RuleFor(c => c.diamond4c.isLabDiamond).NotEmpty();

            RuleFor(c => c.measurement.Depth).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.withLenghtRatio).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.table).NotEmpty().GreaterThan(0) ;
            RuleFor(c => c.measurement.Measurement).NotEmpty().MinimumLength(3);

            RuleFor(c => c.details.Symmetry).NotEmpty();
            RuleFor(c => c.details.Culet).NotEmpty();
            RuleFor(c => c.details.Polish).NotEmpty();
            RuleFor(c => c.details.Girdle).NotEmpty();
            RuleFor(c => c.details.Fluorescence).NotEmpty();

            RuleFor(c => c.hasGIA).NotEmpty();

        }
    }
}
