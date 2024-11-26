using DiamondShop.Application.Commons.Validators.ErrorMessages;
using DiamondShop.Domain.Common.ValueObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces.AdminConfigurations.FrontendDisplays
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
    public class FrontendDisplayConfiguration
    {
        public const string CAROUSEL_FOLDERS = "Carousel";
        public const string Key = "FrontendDisplayConfigurationVer.1";
        public int MaxCarouselImages { get; set; } = 10;
        public int MinCarouselImages { get; set; } = 3;
        public int DisplayTimeInSeconds { get; set; } = 5;
    }
    public interface FrontendDisplayRulesConfigurationService : IBaseConfigurationService<FrontendDisplayConfiguration>
    {
        Task<List<Media>> GetAllCarouselImages();
    }
}
