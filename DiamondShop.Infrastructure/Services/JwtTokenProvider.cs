using BeatvisionRemake.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeatvisionRemake.Infrastructure.Services
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        public const string JWT_SINGING_KEY= "adfsasdfadsfasdfasdfasdfsadfsdfdasffasdds";
        //public (string accessToken, DateTime expiredDate) GenerateAccessToken(User user)
        //{
        //    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SINGING_KEY));
        //    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        //    var expiredTime = DateTime.Now.AddMinutes(100);
        //    var tokeOptions = new JwtSecurityToken(
        //        claims: GetUserClaims(user),
        //        expires: expiredTime,
        //        signingCredentials: signinCredentials
        //    ); ;

        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //    return (tokenString, expiredTime);
        //}

        //public (string refreshToken, DateTime expiredDate) GenerateRefreshToken(User user)
        //{
        //    return (Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
        //        DateTime.Now.AddHours(7));
        //}

        //public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_SINGING_KEY)),
        //        ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
        //    };
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken securityToken;
        //    ClaimsPrincipal principal;
        //    try
        //    {
        //        principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //    }
        //    catch (SecurityTokenValidationException ex)
        //    {
        //        return null;
        //    }
        //    var jwtSecurityToken = securityToken as JwtSecurityToken;
        //    if (jwtSecurityToken == null ||
        //        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        throw new SecurityTokenException("Invalid token");

        //    return principal;
        //}

        //public IEnumerable<Claim> GetUserClaims(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim("email", user.Email),
        //        new Claim("fullname", user.Name)
        //    };
        //    return claims;
        //}
    }
}
