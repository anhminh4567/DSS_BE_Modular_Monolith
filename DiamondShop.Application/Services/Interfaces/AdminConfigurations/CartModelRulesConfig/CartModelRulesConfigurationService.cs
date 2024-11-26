using DiamondShop.Application.Commons.Validators.ErrorMessages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.CartModelRulesConfig
{
    public class CartModelRulesConfigurationValidator : AbstractValidator<CartModelRulesConfiguration>
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
    public class CartModelRulesConfiguration
    {
        public const string key = "CartModelRulesConfigurationVer.1";
        public int MaxItemPerCart { get; set; } = 10;
    }
    public interface CartModelRulesConfigurationService : IBaseConfigurationService<CartModelRulesConfiguration>
    {
    }
}
