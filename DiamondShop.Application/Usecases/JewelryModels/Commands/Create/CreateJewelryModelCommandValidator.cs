using FluentValidation;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public class CreateJewelryModelCommandValidator : AbstractValidator<CreateJewelryModelCommand>
    {
        public CreateJewelryModelCommandValidator()
        {
            RuleForEach(c => c.SideDiamondSpecs)
                .NotEmpty()
                .When(c => c.SideDiamondSpecs != null);

            RuleForEach(c => c.MainDiamondSpecs)
                .NotEmpty()
                .When(c => c.MainDiamondSpecs != null);

            RuleFor(c => c.ModelSpec)
                .NotEmpty()
                .ChildRules(items =>
                {
                    items.RuleFor(c => c.Name)
                        .NotEmpty();
                    
                    items.RuleFor(c => c.Width)
                        .GreaterThan(0).When(c => c.Width.HasValue);
                    
                    items.RuleFor(c => c.Length)
                        .GreaterThan(0).When(c => c.Length.HasValue);

                    items.RuleFor(c => c.IsEngravable)
                        .NotEmpty();

                    items.RuleFor(c => c.IsRhodiumFinish)
                        .NotEmpty();

                    items.RuleFor(c => c.BackType)
                        .IsInEnum();

                    items.RuleFor(c => c.ClaspType)
                        .IsInEnum();

                    items.RuleFor(c => c.ChainType)
                        .IsInEnum();
                });
        }
    }
}
