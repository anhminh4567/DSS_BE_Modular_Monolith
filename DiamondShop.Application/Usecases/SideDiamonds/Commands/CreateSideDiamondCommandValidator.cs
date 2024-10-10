using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands
{
    public class CreateSideDiamondCommandValidator : AbstractValidator<CreateSideDiamondCommand>
    {
        public CreateSideDiamondCommandValidator()
        {
            RuleFor(c => c.ModelId)
                .NotEmpty();

            RuleFor(c => c.SideDiamondSpecs)
                .NotEmpty()
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.ColorMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ColorMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ColorMin <= (int)max).WithMessage((e) => $"ColorMin '{e.ColorMin}' needs to be smaller than ColorMax '{e.ColorMax}'");

                    p.RuleFor(p => p.ClarityMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ClarityMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ClarityMin <= (int)max).WithMessage((e) => $"ClarityMin '{e.ClarityMin}' needs to be smaller than ClarityMax '{e.ClarityMax}'");

                    p.RuleFor(p => p.SettingType)
                        .IsInEnum();

                    p.RuleFor(p => p.ShapeId)
                        .NotEmpty();

                    p.RuleForEach(p => p.OptSpecs)
                        .ChildRules(k =>
                        {
                            k.RuleFor(k => k.CaratWeight)
                                .NotNull()
                                .GreaterThan(0);

                            k.RuleFor(k => k.Quantity)
                                .NotNull()
                                .GreaterThan(0);

                        });
                });
        }
    }
}
