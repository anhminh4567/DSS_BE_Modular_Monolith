using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.JewelryModels
{
    public class JewelryModelRulesConfigurationValidator : AbstractValidator<JewelryModelRules>
    {
        public JewelryModelRulesConfigurationValidator()
        {
            RuleFor(x => x.MaximumMainDiamond)
                .NotEmpty()
                    .WithNotEmptyMessage()
                .GreaterThan(0)
                    .WithGreaterThanMessage()
                .LessThanOrEqualTo(5)
                    .WithLessThanOrEqualMessage();
            RuleFor(x => x.MaximumSideDiamondOption)
                .NotEmpty().WithNotEmptyMessage()
                .GreaterThan(0).WithGreaterThanMessage()
                .LessThanOrEqualTo(5).WithLessThanOrEqualMessage();

        }
    }
}
