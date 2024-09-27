using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System.Globalization;

namespace DiamondShop.Application.Usecases.Promotions.Commands.Create
{
    public class CreatePromotionCommandValidator : AbstractValidator<CreatePromotionCommand>
    {
        public CreatePromotionCommandValidator()
        {
            RuleFor(x => x.name).NotEmpty();
            RuleFor(x => x.description).NotEmpty();
            RuleFor(x => x.RedemptionMode).IsInEnum();

            RuleFor(x => x.startDateTime).NotEmpty()
                .Must(BeAValidDate).WithMessage("Invalid Start Date format.");

            RuleFor(x => x.endDateTime).NotEmpty()
                .Must(BeAValidDate).WithMessage("Invalid End Date format.");

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
                .WithMessage("The Start Date must be before the End Date, and the promotion must meet the minimum duration.");
        }

        private bool BeAValidDate(string dateString)
        {
            // Simple date format validation
            return DateTime.TryParseExact(dateString,DateTimeFormatingRules.DateTimeFormat, null,DateTimeStyles.None ,out _);
        }
    }
}
