using DiamondShop.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DiamondShop.Application.Usecases.Diamonds.Commands.Create
{
    public class CreateDiamondCommandValidator : AbstractValidator<CreateDiamondCommand>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        public CreateDiamondCommandValidator(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            var diamondRule= _optionsMonitor.CurrentValue.DiamondRule;
            RuleFor(c => c.shapeId).NotEmpty();
            RuleFor(c => c.diamond4c.Cut).NotEmpty();
            RuleFor(c => c.diamond4c.Color).NotEmpty();
            RuleFor(c => c.diamond4c.Clarity).NotEmpty();
            RuleFor(c => c.diamond4c.Carat).NotEmpty();
            RuleFor(c => c.diamond4c.isLabDiamond).NotNull();
            RuleFor(c => c.diamond4c.Carat).GreaterThanOrEqualTo(diamondRule.SmallestMainDiamondCarat);

            RuleFor(c => c.measurement.Depth).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.withLenghtRatio).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.table).NotEmpty().GreaterThan(0) ;
            RuleFor(c => c.measurement.Measurement).NotEmpty().MinimumLength(3);

            RuleFor(c => c.details.Symmetry).NotEmpty();
            RuleFor(c => c.details.Culet).NotEmpty();
            RuleFor(c => c.details.Polish).NotEmpty();
            RuleFor(c => c.details.Girdle).NotEmpty();
            RuleFor(c => c.details.Fluorescence).NotEmpty();


        }
    }
}
