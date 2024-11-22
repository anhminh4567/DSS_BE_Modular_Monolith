
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<string>> Register(string email, string password, FullName fullname,bool emailEnabled = false, CancellationToken cancellationToken = default);
        Task<Result<(string identityId, FullName fullName, string email)>> ExternalRegister(CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResultDto>> Login(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResultDto>> ExternalLogin(CancellationToken cancellationToken = default);
        Task<Result<AuthenticationProperties>> GetProviderAuthProperty(string providerName, string callback_URL, CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResultDto>> LoginStaff(string email, string password, CancellationToken cancellationToken = default);
        
        Task<Result> Logout(string identityID, CancellationToken cancellationToken = default);
        Task<Result> BanAccount(string identityID, CancellationToken cancellationToken = default);
        Task<Result<string>> ConfirmEmail(string identityId, string token);
        Task<Result> SendConfirmEmail(string accountId, CancellationToken cancellationToken = default);
        Task<Result> ChangePassword(string identityId, string oldPassword, string newPassword, CancellationToken cancellationToken = default);

        Task<Result<(string? refreshToken, DateTime? ExpiredDate)>> GetRefreshToken(string identityId, CancellationToken cancellationToken = default);
        Task<Result<ClaimsPrincipal>> GetClaimsPrincipalFromCurrentUserContext(CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResultDto>> RefreshingToken(string refreshToken, List<AccountRole> roles, string email, string identityId, string userId, string fullname, CancellationToken cancellationToken);
        ////
        ////
        Task<PagingResponseDto<Account>> GetAccountPagingIncludeIdentity(string[]? roleIds,string? email, int current = 0, int size = 10, CancellationToken cancellationToken = default);
        Task<Result<Account>> GetAccountDetailIncludeIdentity(AccountId accountId, CancellationToken cancellationToken = default);
        Task<Result<AuthenticationResultDto>> GoogleHandler(string credential, CancellationToken cancellationToken = default);
        Task<Result> DeleteByIdentityUser(string identityId);
    }
}
