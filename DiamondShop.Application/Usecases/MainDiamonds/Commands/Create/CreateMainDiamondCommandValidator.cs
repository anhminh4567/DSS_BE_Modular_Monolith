using FluentValidation;

namespace DiamondShop.Application.Usecases.MainDiamonds.Commands.Create
{
    internal class CreateMainDiamondCommandValidator : AbstractValidator<CreateMainDiamondCommand>
    {
        public CreateMainDiamondCommandValidator()
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();
            
            RuleFor(c => c.MainDiamondSpec)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.SettingType)
                        .IsInEnum();

                    p.RuleFor(p => p.Quantity)
                        .NotNull()
                        .GreaterThan(0);

                    p.RuleFor(p => p.ShapeSpecs)
                        .NotNull();

                    p.RuleForEach(p => p.ShapeSpecs)
                        .ChildRules(k =>
                        {
                            k.RuleFor(k => k.ShapeId)
                                .NotEmpty();

                            k.RuleFor(k => k.CaratFrom)
                                .NotNull()
                                .GreaterThan(0f);

                            k.RuleFor(k => k.CaratTo)
                                .NotEmpty()
                                .Must((spec, to) => spec.CaratFrom <= to).WithMessage((e) => $"CaratFrom '{e.CaratFrom}' needs to be smaller than CaratTo '{e.CaratTo}'");
                        });                      
                });
                

        }
    }
}
