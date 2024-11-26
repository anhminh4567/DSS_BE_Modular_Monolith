using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.CartModelRulesConfig.Models
{
    public class CartModelRulesConfigurationValidator : AbstractValidator<CartModelRules>
    {
        public CartModelRulesConfigurationValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.MaxItemPerCart).NotNull()
                .WithNotEmptyMessage();
            RuleFor(x => x.MaxItemPerCart).GreaterThan(0)
                .WithGreaterThanMessage();
        }
    }
}
