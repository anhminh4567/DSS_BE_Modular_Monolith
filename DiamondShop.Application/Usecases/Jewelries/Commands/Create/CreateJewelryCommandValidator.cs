using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Jewelries.Commands.Create
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
        }
    }
}
