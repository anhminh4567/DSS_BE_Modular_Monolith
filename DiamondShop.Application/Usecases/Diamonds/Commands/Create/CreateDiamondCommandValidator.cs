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
            RuleFor(c => c.diamond4c.Cut).NotEmpty().IsInEnum();
            RuleFor(c => c.diamond4c.Color).NotEmpty().IsInEnum();
            RuleFor(c => c.diamond4c.Clarity).NotEmpty().IsInEnum();
            RuleFor(c => c.diamond4c.isLabDiamond).NotNull();
            RuleFor(c => c.diamond4c.Carat).Cascade(CascadeMode.Stop).NotNull()
                .GreaterThanOrEqualTo(diamondRule.SmallestMainDiamondCarat)
                .Must(c =>
                {
                    var caratToString = c.ToString();
                    var numbersBehindComma = caratToString.Substring(caratToString.IndexOf('.') + 1);
                    if (numbersBehindComma.Length > diamondRule.MainDiamondMaxFractionalNumber)
                        return false;
                    return true;
                }).WithMessage($"main diamond now only allow you to input {diamondRule.MainDiamondMaxFractionalNumber} number(s) in fractional part");
            RuleFor(c => c.measurement.Depth).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.withLenghtRatio).NotEmpty().GreaterThan(0);
            RuleFor(c => c.measurement.table).NotEmpty().GreaterThan(0) ;
            RuleFor(c => c.measurement.Measurement).NotEmpty().MinimumLength(3);

            RuleFor(c => c.details.Symmetry).NotEmpty().IsInEnum();
            RuleFor(c => c.details.Culet).NotEmpty().IsInEnum();
            RuleFor(c => c.details.Polish).NotEmpty().IsInEnum();
            RuleFor(c => c.details.Girdle).NotEmpty().IsInEnum();
            RuleFor(c => c.details.Fluorescence).NotEmpty().IsInEnum();
            RuleFor(c => c.Certificate).IsInEnum()
                .When(x => x.Certificate != null);

        }
    }
}
