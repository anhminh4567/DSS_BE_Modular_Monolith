using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Usecases.Jewelries.Commands
{
    public class CreateJewelryCommandValidator : AbstractValidator<CreateJewelryCommand>
    {
        public CreateJewelryCommandValidator()
        {
            RuleForEach(p => p.attachedDiamondIds)
                .NotEmpty()
                .When(p => p.attachedDiamondIds != null);

            RuleFor(p => p.JewelryRequest)
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.ModelId)
                        .NotEmpty();

                    p.RuleFor(p => p.SizeId)
                        .NotEmpty();
                    
                    p.RuleFor(p => p.MetalId)
                        .NotEmpty();
                });

            RuleForEach(p => p.SideDiamondOptId)
                .NotEmpty();
        }
    }
}
