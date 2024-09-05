using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BeatvisionRemake.Application.Services.Interfaces;

namespace DiamondShop.Infrastructure.Options.Setups
{
    internal class JwtBearerOptionSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtOptions _jwtOptions;

        public JwtBearerOptionSetup(IOptions<JwtOptions> options)
        {
            _jwtOptions = options.Value;
        }

        public void Configure(JwtBearerOptions config)
        {
            var validationParam = config.TokenValidationParameters;
            validationParam.ValidIssuer = _jwtOptions.ValidIssuer;
            validationParam.ValidAudience = _jwtOptions.Audience;

            validationParam.ValidateAudience = false;
            validationParam.ValidateLifetime = true;

            validationParam.ValidateIssuerSigningKey = true;
            validationParam.ValidateIssuer = false;
            validationParam.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

            validationParam.RoleClaimType = IJwtTokenProvider.ROLE_CLAIM_NAME;
            validationParam.NameClaimType = IJwtTokenProvider.EMAIL_CLAIM_NAME;
            config.RequireHttpsMetadata = _jwtOptions.RequireHttpsMetadata;
            config.MetadataAddress = _jwtOptions.MetadataUrl;
            config.SaveToken = true;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}