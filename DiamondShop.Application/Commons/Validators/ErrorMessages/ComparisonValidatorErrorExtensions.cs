using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Validators.ErrorMessages
{
    public static class ComparisonValidatorErrorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithGreaterThanMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' phải lớn hơn {ComparisonValue}";
            });
        }
        public static IRuleBuilderOptions<T, TProperty> WithGreaterThanOrEqualMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' phải lớn hơn hay bằng {ComparisonValue}";
            });
        }

        public static IRuleBuilderOptions<T, TProperty> WithLessThanMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' phải bé hơn {ComparisonValue}";
            });
        }
        public static IRuleBuilderOptions<T, TProperty> WithLessThanOrEqualMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' phải bé hơn hay bằng {ComparisonValue}";
            });
        }

    }
}
