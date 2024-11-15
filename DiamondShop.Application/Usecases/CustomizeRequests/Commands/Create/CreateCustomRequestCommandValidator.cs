using DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.CustomizeRequests.Commands.Create
{
    public class CreateCustomRequestCommandValidator : AbstractValidator<CreateCustomizeRequestCommand>
    {
        public CreateCustomRequestCommandValidator()
        {
            RuleFor(p => p.AccountId).NotEmpty();
            RuleFor(p => p.ModelRequest).ChildRules(
                p =>
                {
                    p.RuleFor(k => k.JewelryModelId).NotEmpty();
                    p.RuleFor(k => k.MetalId).NotEmpty();
                    p.RuleFor(k => k.SizeId).NotEmpty();
                    p.RuleFor(k => k.SideDiamondOptId).NotEmpty();
                    p.RuleForEach(k => k.CustomizeDiamondRequests)
                    .Where(k => k != null)
                    .ChildRules(k =>
                    {
                        k.RuleFor(j => j.DiamondShapeId).NotEmpty();
                        k.RuleFor(j => j.clarityFrom).IsInEnum();
                        k.RuleFor(j => j.clarityTo).IsInEnum();
                        k.RuleFor(j => j.colorFrom).IsInEnum();
                        k.RuleFor(j => j.colorTo).IsInEnum();
                        k.RuleFor(j => j.cutFrom).IsInEnum();
                        k.RuleFor(j => j.cutTo).IsInEnum();
                        k.RuleFor(j => j.caratFrom).NotEmpty();
                        k.RuleFor(j => j.caratTo).NotEmpty();
                        k.RuleFor(j => j).Must(j =>
                        {
                            return j.clarityTo >= j.clarityFrom &&
                            j.colorTo >= j.colorFrom &&
                            j.cutTo >= j.cutFrom &&
                            j.caratTo >= j.caratFrom;
                        });
                    });
                });
        }
    }
}
