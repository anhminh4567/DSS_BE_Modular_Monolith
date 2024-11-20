using Azure.Core;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Validators.ErrorMessages
{
    public static class ValidatorErrorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithNotEmptyMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder,string? fieldName = null)
        {
            if(fieldName == null)
            {
                return ruleBuilder.WithMessage((src, property) =>
                {
                    return "'{PropertyName}' không được để trống";
                });
            }
            return ruleBuilder.WithMessage((src, property) =>
            {
                return $"'{fieldName}' không được để trống";
            });
        }
        public static IRuleBuilderOptions<T, TProperty> WithIsInEnumMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, string? fieldName = null)
        {
            if (fieldName == null)
            {
                return ruleBuilder.WithMessage((src, property) =>
                {
                    return "'{PropertyName}' không nằm trong khoảng giá trị cho phép";
                });
            }
            return ruleBuilder.WithMessage((src, property) =>
            {
                return $"'{fieldName}' hông nằm trong khoảng giá trị cho phép";
            });
        }
    }
}
