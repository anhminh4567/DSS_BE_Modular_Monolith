using DiamondShop.Application.Dtos.Requests.JewelryModels;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.SideDiamonds.Commands.Create
{
    public class CreateModelSideDiamondCommandValidator : AbstractValidator<CreateModelSideDiamondCommand>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public CreateModelSideDiamondCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            RuleFor(c => c.ModelId)
                .NotEmpty();
             
            RuleFor(c => c.SideDiamondSpec)
                .NotEmpty()
                .Must((req) =>
                {
                    var diamondRules = _optionsMonitor.CurrentValue.DiamondRule;
                    var maxSideDiamond = diamondRules.BiggestSideDiamondCarat;
                    var averageCarat = req.CaratWeight / (float)req.Quantity;
                    if (averageCarat > (float)maxSideDiamond)
                    {
                        return false;
                    }
                    return true;
                })
                .WithMessage(JewelryModelErrors.SideDiamond.UnsupportedSideDiamondCaratError.Message)
                .ChildRules(p =>
                {
                    p.RuleFor(p => p.ColorMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ColorMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ColorMin <= (int)max).WithMessage((e) => $"Màu nhỏ nhất (\"{e.ColorMin}\") phải bé hơn màu lớn nhất (\"{e.ColorMax}\")");

                    p.RuleFor(p => p.ClarityMin)
                        .IsInEnum();

                    p.RuleFor(p => p.ClarityMax)
                        .IsInEnum()
                        .Must((spec, max) => (int)spec.ClarityMin <= (int)max).WithMessage((e) => $"Độ trong nhỏ nhất (\"{e.ClarityMin}\") phải bé hơn độ trong lớn nhất (\"{e.ClarityMax}\")");

                    p.RuleFor(p => p.SettingType)
                        .IsInEnum();

                    p.RuleFor(p => p.ShapeId)
                        .NotEmpty();

                    p.RuleFor(p => p.CaratWeight)
                                .NotNull()
                                .GreaterThan(0);

                    p.RuleFor(p => p.Quantity)
                        .NotNull()
                        .GreaterThan(0);

                });
        }
    }
}
