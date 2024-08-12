using DiamondShop.Application.Commons.PipelineBehavior;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiamondShop.Application
{
    public static class DependencyInjections
    {
        public static Assembly CurrentAssembly = typeof(DependencyInjections).Assembly;
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(CurrentAssembly);
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));

            });
            services.AddValidatorsFromAssembly(CurrentAssembly);
            return services;
        }
    }
}
