using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Services;

internal class JwtTokenProvider : IJwtTokenProvider
{
    // public const string JWT_SINGING_KEY= "adfsasdfadsfasdfasdfasdfsadfsdfdasffasdds";
    private readonly JwtOptions _jwtOptions;
    private readonly JwtBearerOptions _jwtBearerOptions;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<JwtTokenProvider> _logger;
    public JwtTokenProvider(IOptions<JwtOptions> jwtOptions, IOptions<JwtBearerOptions> jwtBearerOptions, IDateTimeProvider dateTimeProvider, ILogger<JwtTokenProvider> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _jwtBearerOptions = jwtBearerOptions.Value;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public (string accessToken, DateTime expiredDate) GenerateAccessToken(List<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        // need .ToLocalTime() since .net only work with DateTime.Now to validate token
        var expiredTime = _dateTimeProvider.UtcNow.AddDays(100).ToLocalTime();
        var tokeOptions = new JwtSecurityToken(
            issuer: _jwtBearerOptions.TokenValidationParameters.ValidIssuer,
            claims: claims,
            expires: expiredTime,
            signingCredentials: signinCredentials
        ); 

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return (tokenString, expiredTime);
    }

    public (string refreshToken, DateTime expiredDate) GenerateRefreshToken(string identityId)
    {
        return (Convert.ToBase64String(Guid.NewGuid().ToByteArray()), _dateTimeProvider.UtcNow.AddDays(101));
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        ClaimsPrincipal principal;
        try
        {
            principal = tokenHandler.ValidateToken(token, _jwtBearerOptions.TokenValidationParameters, out securityToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("exception in getting principle in jwt token handler, when trying to parse the principle " +
                "from existing access token, with message: {1} ", ex.Message);
            return null;
        }
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public List<Claim> GetUserClaims(List<AccountRole> roles, string email, string identityid, string userId, string fullname)
    {
        var claims = new List<Claim>();
        foreach (var role in roles)
        {
            claims.Add(new Claim(IJwtTokenProvider.ROLE_CLAIM_NAME, role.Id.Value.ToString()));
        }
        claims.Add(new Claim(IJwtTokenProvider.EMAIL_CLAIM_NAME, email));
        claims.Add(new Claim(IJwtTokenProvider.USERNAME_CLAIM_NAME, fullname));
        claims.Add(new Claim(IJwtTokenProvider.IDENTITY_CLAIM_NAME, identityid));
        claims.Add(new Claim(IJwtTokenProvider.USER_ID_CLAIM_NAME, userId));

        return claims;
    }
}
