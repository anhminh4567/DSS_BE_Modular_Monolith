using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SizeMetals.Commands.Create
{
    public class CreateSizeMetalCommandValidator : AbstractValidator<CreateSizeMetalCommand>
    {
        public CreateSizeMetalCommandValidator() 
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();
            RuleForEach(c => c.MetalSizeSpecs)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.MetalId)
                        .NotEmpty();

                    p.RuleFor(p => p.SizeId)
                        .NotEmpty();

                    p.RuleFor(p => p.Weight)
                        .NotEmpty()
                        .GreaterThan(0f);
                });
        }
    }
}
