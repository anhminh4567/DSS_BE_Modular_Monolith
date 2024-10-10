using FluentValidation;

namespace DiamondShop.Application.Usecases.MainDiamonds.Commands.CompareDiamondShape
{
    public class CompareDiamondShapeCommandValidator : AbstractValidator<CompareDiamondShapeCommand>
    {
        public CompareDiamondShapeCommandValidator()
        {
            RuleFor(p => p.Diamonds)
                .NotEmpty();

            RuleForEach(p => p.Diamonds)
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.DiamondShapeId)
                        .NotEmpty();

                    p.RuleFor(p => p.Carat)
                        .NotEmpty();
                });
        }
    }
}
