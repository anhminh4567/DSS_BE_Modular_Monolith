using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        .NotEmpty();

                    p.RuleFor(p => p.Quantity)
                        .NotEmpty()
                        .GreaterThan(0);

                    p.RuleFor(p => p.ShapeSpecs)
                        .NotNull();

                    p.RuleForEach(p => p.ShapeSpecs)
                        .ChildRules(k =>
                        {
                            k.RuleFor(k => k.ShapeId)
                                .NotEmpty();

                            k.RuleFor(k => k.MainDiamondReqId)
                               .NotEmpty();

                            k.RuleFor(k => k.CaratFrom)
                                .NotEmpty()
                                .GreaterThan(0f);

                            k.RuleFor(k => k.CaratTo)
                                .NotEmpty()
                                .Must((spec,k) => spec.CaratFrom <= k);
                        });                      
                });
                

        }
    }
}
