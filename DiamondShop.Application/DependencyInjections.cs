using DiamondShop.Application.Commons.PipelineBehavior;
using DiamondShop.Domain.Services.Implementations;
using DiamondShop.Domain.Services.interfaces;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiamondShop.Application
{
    public static class DependencyInjections
    {
        public static Assembly CurrentAssembly = typeof(DependencyInjections).Assembly;//Assembly.GetExecutingAssembly();
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
            services.AddMapping(configuration);
            return services;
        }
        private static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddSingleton<IDiamondServices, DiamondServices>();
            services.AddSingleton<IOrderTransactionService, OrderTransactionService>();
            services.AddSingleton<IPromotionServices, PromotionService>();
            services.AddSingleton<IDiscountService, DiscountService>();
            services.AddScoped<IOrderTransactionService, OrderTransactionService>();
            services.AddScoped<ICartModelService, CartModelService>();

            return services;
        }
        private static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration configuration)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(CurrentAssembly);
            config.Default.IgnoreNullValues(true);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
            return services;
        }
    }
}
