using DiamondShop.Application.Commons.PipelineBehavior;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
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
            services.AddDomain(configuration);
            return services;
        }
        private static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddScoped<IDiamondServices, DiamondServices>();
            return services;
        }
    }
}
