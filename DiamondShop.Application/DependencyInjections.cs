using DiamondShop.Application.Commons.PipelineBehavior;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiamondShop.Application
{
    public static class DependencyInjections
    {
        public static Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddValidatorsFromAssembly(CurrentAssembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(CurrentAssembly);
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));

            });
            
            return services;
        }
    }
}
