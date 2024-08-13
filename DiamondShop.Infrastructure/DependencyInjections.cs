using BeatvisionRemake.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DiamondShop.Infrastructure.Options;

namespace DiamondShop.Infrastructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationOptions>(options =>
            {
                configuration.GetSection("Authentication");
            });
            var authOpt = new AuthenticationOptions();
            configuration.GetSection("Authentication").Bind(authOpt);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.Configure<JwtBearerOptions>(config =>
            {
                var validationParam = config.TokenValidationParameters;
                validationParam.ValidIssuer = authOpt.ValidIssuer;
                validationParam.ValidAudience = authOpt.Audience;
                validationParam.ValidateIssuer = false;
                validationParam.ValidateAudience = false;
                validationParam.ValidateLifetime = true;
                validationParam.ValidateIssuerSigningKey = true;
                validationParam.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenProvider.JWT_SINGING_KEY));
                config.RequireHttpsMetadata = authOpt.RequireHttpsMetadata;
                config.MetadataAddress = authOpt.MetadataUrl;
            });

            return services;
        }
    }
}
