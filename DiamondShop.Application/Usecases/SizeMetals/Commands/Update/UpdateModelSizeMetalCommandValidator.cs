using FluentValidation;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Update
{
    public class UpdateModelSizeMetalCommandValidator : AbstractValidator<UpdateModelSizeMetalCommand>
    {
        public UpdateModelSizeMetalCommandValidator()
        {
            RuleFor(p => p.ModelId).NotEmpty();
            RuleFor(p => p.SizeMetals).NotEmpty();
            RuleForEach(p => p.SizeMetals).ChildRules(p =>
            {
                p.RuleFor(k => k.MetalId).NotEmpty();
                p.RuleFor(k => k.SizeId).NotEmpty();
                p.RuleFor(k => k.Weight).GreaterThan(0);
            });
        }
    }
}
