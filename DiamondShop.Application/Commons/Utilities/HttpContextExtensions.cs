using DiamondShop.Application.Services.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Commons.Utilities
{
    public static class HttpContextExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME)?.Value;
        }
        public static string? GetUserIdentityId(this ClaimsPrincipal user)
        {
            return user.FindFirst(IJwtTokenProvider.IDENTITY_CLAIM_NAME)?.Value;
        }
        public static string[]? GetUserRoles(this ClaimsPrincipal user)
        {
            return user.FindAll(IJwtTokenProvider.ROLE_CLAIM_NAME).Select(x => x.Value).ToArray();
        }
        public static class CommonErrors 
        {
            public static Error NotFound => new Error("Không tìm thấy kết nối người dùng");
            public static Error NotAuthenticated => new Error("Người dùng chưa xác thực");
            public static Error NotAuthorized => new Error("Người dùng không có quyền truy cập");
            public static Error Banned => new Error("Người dùng bị cấm tr");
        }

    }
}
