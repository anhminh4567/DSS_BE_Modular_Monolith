using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.JewelryModels.ErrorMessages;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.JewelryModels.Commands.Create
{
    public class CreateJewelryModelCommandValidator : AbstractValidator<CreateJewelryModelCommand>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public CreateJewelryModelCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            RuleFor(c => c.SideDiamondSpecs)
                .Must(c => c.Count <= optionsMonitor.CurrentValue.JewelryModelRules.MaximumSideDiamondOption)
                .WithMessage(JewelryModelErrors.SideDiamond.ModelMaximumOptionError(optionsMonitor.CurrentValue.JewelryModelRules.MaximumSideDiamondOption).Message)
                .When(c => c.SideDiamondSpecs != null);

            RuleFor(c => c.MainDiamondSpecs)
                .Must(c =>
                {
                    int quantity = 0;
                    foreach (var main in c)
                    {
                        quantity += main.Quantity;
                    }
                    return quantity <= optionsMonitor.CurrentValue.JewelryModelRules.MaximumMainDiamond;
                })
                .WithMessage(JewelryModelErrors.MainDiamond.ModelMaximumOptionError(optionsMonitor.CurrentValue.JewelryModelRules.MaximumSideDiamondOption).Message)
                .When(c => c.MainDiamondSpecs != null);

            RuleForEach(c => c.SideDiamondSpecs)
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
                .When(c => c.SideDiamondSpecs != null);

            RuleForEach(c => c.MainDiamondSpecs)
                .NotEmpty()
                .When(c => c.MainDiamondSpecs != null);

            RuleFor(c => c.ModelSpec)
                .NotEmpty()
                .ChildRules(items =>
                {
                    items.RuleFor(c => c.Name)
                        .NotEmpty();

                    items.RuleFor(c => c.Code)
                        .NotEmpty();

                    items.RuleFor(c => c.Width)
                        .GreaterThan(0).When(c => c.Width.HasValue);

                    items.RuleFor(c => c.Length)
                        .GreaterThan(0).When(c => c.Length.HasValue);

                    items.RuleFor(c => c.IsEngravable)
                        .NotEmpty();

                    //items.RuleFor(c => c.IsRhodiumFinish)
                    //    .NotEmpty();

                    items.RuleFor(c => c.BackType)
                        .IsInEnum();

                    items.RuleFor(c => c.ClaspType)
                        .IsInEnum();

                    items.RuleFor(c => c.ChainType)
                        .IsInEnum();
                });
        }
    }
}
