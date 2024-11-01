using DiamondShop.Application.Dtos.Requests.JewelryModels;
using FluentValidation;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public class CreateSizeMetalCommandValidator : AbstractValidator<CreateSizeMetalCommand>
    {
        public CreateSizeMetalCommandValidator()
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();
            RuleFor(c => c.MetalSizeSpec)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.MetalId)
                        .NotEmpty();

                    p.RuleFor(p => p.SizeId)
                        .NotEmpty();

                    p.RuleFor(p => p.Weight)
                        .NotNull()
                        .GreaterThan(0f);
                })
                ;
        }
    }
}
