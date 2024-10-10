using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.RoleAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IJwtTokenProvider
    {
        public const string ROLE_CLAIM_NAME = "Roles";
        public const string USERNAME_CLAIM_NAME = "Name";
        public const string EMAIL_CLAIM_NAME = "Email";
        public const string IDENTITY_CLAIM_NAME = "IdentityId";
        public const string USER_ID_CLAIM_NAME = "UserId";
        (string accessToken, DateTime expiredDate) GenerateAccessToken(List<Claim> claims);
        (string refreshToken, DateTime expiredDate) GenerateRefreshToken(string identityId);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        List<Claim> GetUserClaims(List<AccountRole> roles, string email, string identityId, string userId, string fullname);
    }
}
