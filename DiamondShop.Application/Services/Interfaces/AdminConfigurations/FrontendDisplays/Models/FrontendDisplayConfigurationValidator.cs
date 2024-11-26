using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.BusinessRules;
using FluentValidation;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays.Models
{
    public class FrontendDisplayConfigurationValidator : AbstractValidator<FrontendDisplayConfiguration>
    {
        public FrontendDisplayConfigurationValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.MaxCarouselImages).NotNull()
                .WithNotEmptyMessage();
            RuleFor(x => x.MinCarouselImages).NotNull()
                .WithNotEmptyMessage();
            RuleFor(x => x.DisplayTimeInSeconds).NotNull()
                .WithNotEmptyMessage();
            RuleFor(x => x.MaxCarouselImages).GreaterThan(0)
                .WithGreaterThanMessage();
            RuleFor(x => x.MinCarouselImages).GreaterThan(0)
                .WithGreaterThanMessage();
            RuleFor(x => x.MinCarouselImages).LessThan(x => x.MaxCarouselImages)
                .WithLessThanMessage();
            RuleFor(x => x.DisplayTimeInSeconds).GreaterThan(1)
                .WithGreaterThanMessage();
        }
    }
}
