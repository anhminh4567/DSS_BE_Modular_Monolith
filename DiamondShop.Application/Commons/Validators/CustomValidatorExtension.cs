using DiamondShop.Application.Commons.Utilities;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Validators
{
    public static class CustomValidatorExtension
    {
        public static IRuleBuilderOptions<T, string> ValidDate<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("Invalid date format.");
        }
        public static IRuleBuilderOptions<T, string> ValidDateGreaterThanUTCNow<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty()
                .Must(DateTimeUtil.BeAValidDate).WithMessage("Invalid date format.")
                .Must(DateTimeUtil.BeGreaterThanUTCNow).WithMessage("Date must be greater than the current UTC time.");
        }
        public static IRuleBuilderOptions<T, T> ValidStartEndDate<T>(this IRuleBuilder<T, T> ruleBuilder, Func<T, string> startDateSelector, Func<T, string> endDateSelector)
        {
            return ruleBuilder
                .Must((command, _) =>
                {
                    var startDateStr = startDateSelector(command);
                    var endDateStr = endDateSelector(command);

                    var isValidStart = DateTime.TryParseExact(startDateStr, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime startDate);
                    var isValidEnd = DateTime.TryParseExact(endDateStr, DateTimeFormatingRules.DateTimeFormat, null, DateTimeStyles.None, out DateTime endDate);

                    if (isValidStart && isValidEnd)
                    {
                        return startDate < endDate;
                    }
                    return false;
                })
                .WithMessage("The Start Date must be before the End Date, and the promotion must meet the minimum duration.");
        }
    }
}
