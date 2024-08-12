
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BeatvisionRemake.Application.Services.Interfaces
{
    public interface IJwtTokenProvider
    {
        //(string accessToken, DateTime expiredDate) GenerateAccessToken(User user);
        //(string refreshToken, DateTime expiredDate) GenerateRefreshToken(User user);
        //ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        //IEnumerable<Claim> GetUserClaims(User user);
    }
}
