using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Application.Commons.Validators;
using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System.Globalization;

namespace DiamondShop.Application.Usecases.Promotions.Commands.Create
{
    public class CreatePromotionCommandValidator : AbstractValidator<CreatePromotionCommand>
    {
        public CreatePromotionCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                    .WithNotEmptyMessage();
            RuleFor(x => x.description)
                .NotEmpty()
                   .WithNotEmptyMessage();
            RuleFor(x => x.RedemptionMode)
                .IsInEnum()
                    .WithIsInEnumMessage();

            RuleFor(x => x.startDateTime)
                .NotEmpty()
                    .WithNotEmptyMessage()
                .ValidDate()
                .When(x => x.startDateTime != null);

            RuleFor(x => x.endDateTime)
                .NotEmpty()
                    .WithNotEmptyMessage()
                .ValidDate()
                .When(x => x.endDateTime != null);
            // Compare the dates after parsing
            RuleFor(x => x)
                .Must((command) =>
                {
                    var isValidStart = DateTime.TryParseExact(command.startDateTime, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime startDate);
                    var isValidEnd = DateTime.TryParseExact(command.endDateTime,DateTimeFormatingRules.DateTimeFormat, null,DateTimeStyles.None, out DateTime endDate);
                    if (isValidStart && isValidEnd)
                    {
                        return startDate < endDate;//&& (endDate - startDate).TotalDays >= settings.MinimumPromotionDuration;
                    }
                    return false;
                })
                .WithMessage("ngày bắt đầu phải trước kết thúc");
        }


    }
}
