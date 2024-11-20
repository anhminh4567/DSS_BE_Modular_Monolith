using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
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
            RuleFor(c => c.shapeId).NotEmpty().WithNotEmptyMessage();
            RuleFor(c => c.diamond4c.Cut).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.diamond4c.Color).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.diamond4c.Clarity).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.diamond4c.isLabDiamond).NotNull().WithNotEmptyMessage();
            RuleFor(c => c.diamond4c.Carat).Cascade(CascadeMode.Stop).NotNull().WithNotEmptyMessage()
                .GreaterThanOrEqualTo(diamondRule.SmallestMainDiamondCarat)
                .Must(c =>
                {
                    var caratToString = c.ToString();
                    var numbersBehindComma = caratToString.Substring(caratToString.IndexOf('.') + 1);
                    if (numbersBehindComma.Length > diamondRule.MainDiamondMaxFractionalNumber)
                        return false;
                    return true;
                }).WithMessage($"kim cương chính chỉ có thể để input {diamondRule.MainDiamondMaxFractionalNumber} (s) phần thập phân");
            RuleFor(c => c.measurement.Depth).NotEmpty().WithNotEmptyMessage().GreaterThan(0);
            RuleFor(c => c.measurement.withLenghtRatio).NotEmpty().WithNotEmptyMessage().GreaterThan(0);
            RuleFor(c => c.measurement.table).NotEmpty().WithNotEmptyMessage().GreaterThan(0) ;
            RuleFor(c => c.measurement.Measurement).NotEmpty().WithNotEmptyMessage().MinimumLength(3);
          
            RuleFor(c => c.details.Symmetry).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.details.Culet).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.details.Polish).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.details.Girdle).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.details.Fluorescence).NotEmpty().WithNotEmptyMessage().IsInEnum().WithIsInEnumMessage();
            RuleFor(c => c.Certificate).IsInEnum()
                .When(x => x.Certificate != null);
            RuleFor(c => c.priceOffset).ValidNumberFraction()
                .Must(c => c >= diamondRule.MinPriceOffset && c <= diamondRule.MaxPriceOffset)
                .WithMessage($"giá offset phải nằm trong khoảng {diamondRule.MinPriceOffset} và  {diamondRule.MaxPriceOffset}, đây là business rule");
        }
    }
}
