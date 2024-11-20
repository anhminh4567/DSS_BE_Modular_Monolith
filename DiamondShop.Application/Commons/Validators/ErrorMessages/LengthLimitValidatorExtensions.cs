using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Validators.ErrorMessages
{
    public static class LengthLimitValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithMaxLenghtMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' có chiều dài bé hơn hay bằng {MaxLength}";
            });
        }
        public static IRuleBuilderOptions<T, TProperty> WithMinLenghtMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.WithMessage((src, property) =>
            {
                return "'{PropertyName}' có chiều dài lớn hơn hay bằng {MinLength}";
            });
        }
    }
}
