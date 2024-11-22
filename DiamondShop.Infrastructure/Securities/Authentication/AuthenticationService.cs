using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Carts.Commands.ValidateFromJson;
using DiamondShop.Commons;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ErrorMessages;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Databases;
using DiamondShop.Infrastructure.Identity.Models;
using DiamondShop.Infrastructure.Options;
using DiamondShop.Infrastructure.Services;
using FluentEmail.Core;
using FluentResults;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium.DevTools.V127.WebAudio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Securities.Authentication
{
    internal class AuthenticationService : DiamondShop.Application.Services.Interfaces.IAuthenticationService
    {
        private readonly CustomRoleManager _roleManager;
        private readonly CustomSigninManager _signinManager;
        private readonly CustomUserManager _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;
        private readonly DiamondShopDbContext _dbContext;
        private const string BEARER_HEADER = "Bearer ";

        public AuthenticationService(CustomRoleManager roleManager, CustomSigninManager signinManager, CustomUserManager userManager, IUnitOfWork unitOfWork, ILogger<AuthenticationService> logger, IJwtTokenProvider jwtTokenProvider, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IHttpContextAccessor contextAccessor, IDateTimeProvider dateTimeProvider, IMemoryCache cache, IEmailService emailService, DiamondShopDbContext dbContext)
        {
            _roleManager = roleManager;
            _signinManager = signinManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtTokenProvider = jwtTokenProvider;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _contextAccessor = contextAccessor;
            _dateTimeProvider = dateTimeProvider;
            _cache = cache;
            _emailService = emailService;
            _dbContext = dbContext;
        }

        private bool CheckIfUserIsValidToLogin(CustomIdentityUser user)
        {
            if (user is null)
                return false;
            if (!user.LockoutEnabled)
                return false;
            return true;
        }
        public async Task<Result<AuthenticationResultDto>> ExternalLogin(CancellationToken cancellationToken = default)
        {
            var info = await _signinManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Result.Fail(new NotFoundError("not found user principle"));
            if ((info.Principal.Claims.FirstOrDefault(c => c.Type == "FAILURE") is not null))
                return Result.Fail("Cannot call request to user infor in google , try again later");
            var getUserByEmail = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (await _userManager.IsLockedOutAsync(getUserByEmail) is true)
                return Result.Fail("cannot login, you are locked out");
            if (getUserByEmail == null)
                return Result.Fail(new NotFoundError());
            if (CheckIfUserIsValidToLogin(getUserByEmail))
            {
                return Result.Fail("user is lock out,contact admin to unlock");
            }
            var getCustomer = await _accountRepository.GetByIdentityId(getUserByEmail.Id, cancellationToken);
            if (getCustomer is null)
                return Result.Fail(new NotFoundError());
            //getCustomer.Roles.First(r => r.Id != AccountRole.Customer.Id);
            var authTokenDto = await GenerateTokenForUser(getCustomer.Roles, getCustomer.Email, getCustomer.IdentityId, getCustomer.Id.Value, getCustomer.FullName.Value);
            return Result.Ok(authTokenDto);
        }

        public async Task<Result<(string identityId, FullName fullName, string email)>> ExternalRegister(CancellationToken cancellationToken = default)
        {
            var info = await _signinManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Result.Fail(new NotFoundError("not found user principle"));
            if ((info.Principal.Claims.FirstOrDefault(c => c.Type == "FAILURE") is not null))
            {
                return Result.Fail("Cannot call request to user infor in google , try again later");
            }
            var tryLogin = await _signinManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (tryLogin.Succeeded)
                return Result.Fail(new ConflictError("user already exist"));
            var getEmail = info.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_EMAIL_CLAIM_NAME);
            var getUsername = info.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_USERNAME_CLAIM_NAME);
            var getProfileImage = info.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_PROFILE_IMAGE_CLAIM_NAME);
            ArgumentNullException.ThrowIfNull(getEmail);
            ArgumentNullException.ThrowIfNull(getUsername);
            var identity = new CustomIdentityUser()
            {
                Email = getEmail,
                UserName = getEmail,
                EmailConfirmed = false,
            };
            var createResult = await _userManager.CreateAsync(identity);
            await _userManager.SetLockoutEnabledAsync(identity, false);
            if (createResult.Succeeded is false)
                return Result.Fail("fail to create identity");
            var createLogin = await _userManager.AddLoginAsync(identity, info);
            if (createLogin.Succeeded is false)
                return Result.Fail("fail to create login");
            FullName fullname;
            string[] splitedName = getUsername.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (splitedName.Length >= 2)
            {
                //string[] lastNameArray = (string[]) splitedName.Clone();
                // Array.Copy(splitedName,1,lastNameArray,0,1);
                //splitedName.CopyTo(lastNameArray, 1);lastNameArray.ToString()
                fullname = FullName.Create(splitedName[0], splitedName[1]);
            }
            else
            {
                fullname = FullName.Create(splitedName[0], "");
            }
            return (identity.Id, fullname, getEmail);
        }



        public async Task<Result<AuthenticationProperties>> GetProviderAuthProperty(string providerName, string callback_URL, CancellationToken cancellationToken = default)
        {
            var getAuthProviderSchemes = await _signinManager.GetExternalAuthenticationSchemesAsync();
            bool isSchemeExist = false;
            foreach (var authScheme in getAuthProviderSchemes)
            {
                if (string.Equals(authScheme.Name, providerName))
                {
                    isSchemeExist = true;
                    break;
                }
            }
            if (isSchemeExist is false)
            {
                return Result.Fail(new NotFoundError("not found the login requested"));
            }
            AuthenticationProperties authProperties = _signinManager.ConfigureExternalAuthenticationProperties(providerName, callback_URL);
            return Result.Ok(authProperties);
        }



        public async Task<Result<AuthenticationResultDto>> Login(string email, string password, CancellationToken cancellationToken = default)
        {
            var loginValidateResult = await PasswordLoginValidation(email, password);
            if (loginValidateResult.IsFailed)
            {
                return Result.Fail(loginValidateResult.Errors);
            }
            var userIdentity = loginValidateResult.Value;
            if (CheckIfUserIsValidToLogin(userIdentity))
            {
                return Result.Fail(AccountErrors.LockAccount);
            }
            var getCustomer = await _accountRepository.GetByIdentityId(userIdentity.Id, cancellationToken);
            // getCustomer.Roles.First(r => r.Id == AccountRole.Customer.Id);
            List<AccountRole> toAccountRole = getCustomer.Roles.Select(r => (AccountRole)r).ToList();
            var authTokenDto = await GenerateTokenForUser(toAccountRole, getCustomer.Email, getCustomer.IdentityId, getCustomer.Id.Value, getCustomer.FullName.Value, cancellationToken);
            return Result.Ok(authTokenDto);
        }
        private async Task<AuthenticationResultDto> GenerateTokenForUser(List<AccountRole> roles, string email, string identityId, string userId, string fullname, CancellationToken cancellationToken = default)
        {
            List<Claim> userClaim = _jwtTokenProvider.GetUserClaims(roles, email, identityId, userId, fullname);
            var accTokenResult = _jwtTokenProvider.GenerateAccessToken(userClaim);
            var refreshTokenResult = _jwtTokenProvider.GenerateRefreshToken(identityId);
            // Save Refresh Token
            await _userManager.SetRefreshTokenAsync(identityId, refreshTokenResult.refreshToken, refreshTokenResult.expiredDate, cancellationToken);
            // the inside function already have saveChanges() on db level
            //await _unitOfWork.SaveChangesAsync();
            return new AuthenticationResultDto(
                accessToken: accTokenResult.accessToken,
                expiredAccess: accTokenResult.expiredDate,
                refreshToken: refreshTokenResult.refreshToken,
                expiredRefresh: refreshTokenResult.expiredDate
            );
        }
        public async Task<Result> Logout(string identityId, CancellationToken cancellationToken = default)
        {
            await _userManager.RemoveRefreshTokenAsync(identityId, cancellationToken);
            return Result.Ok();
        }

        public async Task<Result<string>> Register(string email, string password, FullName fullname, bool emailEnabled = false, CancellationToken cancellationToken = default)
        {
            if ((await _userManager.FindByEmailAsync(email)) is not null)
            {
                return Result.Fail(AccountErrors.Register.UserExist);
            }
            var identity = new CustomIdentityUser();
            identity.Email = email;
            identity.UserName = email;
            identity.LockoutEnabled = false;
            identity.EmailConfirmed = emailEnabled;
            var result = await _userManager.CreateAsync(identity, password);
            await _userManager.SetLockoutEnabledAsync(identity, false);
            if (result.Succeeded is false)
            {
                var errDict = new Dictionary<string, object>();
                foreach (var err in result.Errors)
                {
                    errDict.Add(err.Code, err.Description);
                }
                return Result.Fail(new ValidationError("Password Error", errDict));
            }
            return Result.Ok(identity.Id);

        }



        public async Task<Result<AuthenticationResultDto>> LoginStaff(string email, string password, CancellationToken cancellationToken = default)
        {
            var loginValidateResult = await PasswordLoginValidation(email, password);
            if (loginValidateResult.IsFailed)
            {
                return Result.Fail(loginValidateResult.Errors);
            }
            var userIdentity = loginValidateResult.Value;
            if (CheckIfUserIsValidToLogin(userIdentity))
            {
                return Result.Fail("user is lock out,contact admin to unlock");
            }
            var getStaff = await _accountRepository.GetByIdentityId(userIdentity.Id, cancellationToken);
            if (getStaff is null)
                return Result.Fail(new NotFoundError("staff not found"));

            var authTokenDto = await GenerateTokenForUser(getStaff.Roles, getStaff.Email, getStaff.IdentityId, getStaff.Id.Value, getStaff.FullName.Value, cancellationToken);
            return Result.Ok(authTokenDto);
            // throw new NotImplementedException();
        }
        private async Task<Result<CustomIdentityUser>> PasswordLoginValidation(string email, string password)
        {
            var tryGetAccount = await _userManager.FindByEmailAsync(email);
            if (tryGetAccount is null)
            {
                return Result.Fail(new NotFoundError("account Not Found"));
            }
            if (await _userManager.CheckPasswordAsync(tryGetAccount, password) is false)
                return Result.Fail(new Error("password not match"));
            if (await _userManager.IsLockedOutAsync(tryGetAccount) is true)
                return Result.Fail("can't sign, you might have been block ");
            return Result.Ok(tryGetAccount);
        }
        public async Task<Result<(string? refreshToken, DateTime? ExpiredDate)>> GetRefreshToken(string identityId, CancellationToken cancellationToken = default)
        {
            var userIdentity = await _userManager.FindByIdAsync(identityId);
            if (userIdentity is null)
                return Result.Fail(new NotFoundError());
            if (CheckIfUserIsValidToLogin(userIdentity))
            {
                return Result.Fail("user is lock out,contact admin to unlock");
            }
            var getIdentity = await _userManager.GetRefreshTokenAsync(identityId, cancellationToken);
            if (getIdentity.refreshToken is null)
                return Result.Fail("no token found");
            return Result.Ok((getIdentity.refreshToken, getIdentity.expiredTime));
            //throw new NotImplementedException();
        }

        public async Task<Result<ClaimsPrincipal>> GetClaimsPrincipalFromCurrentUserContext(CancellationToken cancellationToken = default)
        {
            var httpContext = _contextAccessor.HttpContext;
            var accessToken = httpContext.Request.Headers.Authorization.ToString().Substring(BEARER_HEADER.Length);
            if (accessToken.IsNullOrEmpty())
                return Result.Fail("no access token found, login to get it");
            ClaimsPrincipal? tryGetClaimFromAccessToken = _jwtTokenProvider.GetPrincipalFromExpiredToken(accessToken);
            if (tryGetClaimFromAccessToken == null)
                return Result.Fail("cannot get principle from the token, try login again");
            return Result.Ok(tryGetClaimFromAccessToken);
        }

        public async Task<Result<AuthenticationResultDto>> RefreshingToken(string refreshToken, List<AccountRole> roles, string email, string identityId, string userId, string fullname, CancellationToken cancellationToken)
        {
            var getUserRefreshTokenResul = await GetRefreshToken(identityId, cancellationToken);
            if (getUserRefreshTokenResul.IsFailed)
                return Result.Fail(getUserRefreshTokenResul.Errors);

            (string? returnedToken, DateTime? expiredTime) = getUserRefreshTokenResul.Value;
            if (refreshToken == returnedToken)
            {
                // if date time now is greater than expired time, means pass the expired or equal
                if (DateTime.Compare(_dateTimeProvider.UtcNow, expiredTime.Value) >= 0)
                {
                    Result.Fail("Token Expired, Login again");
                }
                var claims = _jwtTokenProvider.GetUserClaims(roles, email, identityId, userId, fullname);
                var newAccToken = _jwtTokenProvider.GenerateAccessToken(claims);
                var newRefreshToken = _jwtTokenProvider.GenerateRefreshToken(identityId);
                //save the new token before return it
                await _userManager.SetRefreshTokenAsync(identityId, newRefreshToken.refreshToken, newRefreshToken.expiredDate, cancellationToken);
                return Result.Ok(new AuthenticationResultDto
                (
                    accessToken: newAccToken.accessToken,
                    expiredAccess: newAccToken.expiredDate,
                    refreshToken: newRefreshToken.refreshToken,
                    expiredRefresh: newRefreshToken.expiredDate
                ));
            }
            return Result.Fail("Token not match");
        }

        public async Task<Result> BanAccount(string identityID, CancellationToken cancellationToken = default)
        {
            var tryGetAccount = await _userManager.FindByIdAsync(identityID);
            if (tryGetAccount is null)
                return Result.Fail(new NotFoundError()); ;
            tryGetAccount.LockoutEnabled = !tryGetAccount.LockoutEnabled;
            await _userManager.UpdateSecurityStampAsync(tryGetAccount);
            await _userManager.UpdateAsync(tryGetAccount);
            return Result.Ok();
        }
        public async Task<Result<string>> ConfirmEmail(string accountId, string token)
        {
            var getAccount = await _accountRepository.GetById(AccountId.Parse(accountId));
            if (getAccount is null)
                return Result.Fail(new NotFoundError("user with this email not found to confirm"));
            var getUser = await _userManager.FindByIdAsync(getAccount.IdentityId);
            if (getUser is null)
                return Result.Fail(new NotFoundError("user with this email not found to confirm"));
            var codeDecodedBytes = WebEncoders.Base64UrlDecode(token);
            var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
            var checkResult = await _userManager.ConfirmEmailAsync(getUser, codeDecoded);
            if (checkResult.Succeeded is false)
                return Result.Fail(new ConflictError("unknown token might expired or invalid token"));
            return Result.Ok("Success");
        }
        public async Task<Result> ChangePassword(string identityId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var tryGetAccount = await _userManager.FindByIdAsync(identityId);
            if (tryGetAccount is null)
                return Result.Fail(new NotFoundError()); ;
            var result = await _userManager.ChangePasswordAsync(tryGetAccount, oldPassword, newPassword);
            if (result.Succeeded is false)
            {
                Dictionary<string, object> errors = new();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Code,new List<object> { "lỗi thay đổi password, hãy chắc ràng password " } );
                }
                return Result.Fail(new ValidationError("lõi thay đỏi password", errors));
            }
            return Result.Ok();
        }
        public async Task<Result> SendConfirmEmail(string accountId, CancellationToken cancellationToken = default)
        {
            var parsedId = AccountId.Parse(accountId);
            var getUserAccount = await _accountRepository.GetById(parsedId, cancellationToken);
            if (getUserAccount is null)
                return Result.Fail(new NotFoundError());
            var getUserIdentity = await _userManager.FindByIdAsync(getUserAccount.IdentityId);
            if (getUserIdentity is null)
                return Result.Fail(new NotFoundError());
            var generateToken = await _userManager.GenerateEmailConfirmationTokenAsync(getUserIdentity);
            generateToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(generateToken));
            return await _emailService.SendConfirmAccountEmail(getUserAccount, generateToken, cancellationToken);
        }

        public async Task<PagingResponseDto<Account>> GetAccountPagingIncludeIdentity(string[]? roleIds,string? email, int current = 0, int size = 10, CancellationToken cancellationToken = default)
        {
            //throw new NotImplementedException();
            var trueCurrent = current * size;
            List<Account> result = new();
            int total = 0;

            var roleQuery = _accountRoleRepository.GetQuery();
            if (roleIds != null && roleIds.Count() > 0)
            {
                var parsedRolesId = roleIds.Select(x => AccountRoleId.Parse(x)).ToList();
                roleQuery = _accountRoleRepository.QueryFilter(roleQuery, x => parsedRolesId.Contains(x.Id));
            }
            roleQuery = _accountRoleRepository.QueryInclude(roleQuery, x => x.Accounts);
            var accountQuery = roleQuery.SelectMany(x => x.Accounts);
            if(email is not null)
                accountQuery = accountQuery.Where(x => x.Email.Contains(email));
            result = accountQuery
                .Distinct()
                .Include(x => x.Roles)
                .AsSplitQuery()
                .Skip(trueCurrent)
                .Take(size)
                .ToList();
            
            var identityIds = result.Select(a => a.IdentityId).Distinct().ToList();

            var customIdentityUsers = await _userManager.Users
                .Where(u => identityIds.Contains(u.Id))
                .ToListAsync();
            //var customIdentityUsers = await _dbContext.Users
            //    .Where(u => identityIds.Contains(u.Id))
            //    .ToListAsync();

            result.ForEach(x => x.UserIdentity = customIdentityUsers.FirstOrDefault(u => u.Id == x.IdentityId));

            total = roleQuery.SelectMany(x => x.Accounts).Count();

            return new PagingResponseDto<Account>(
                TotalPage: (int)Math.Ceiling((decimal)total / (decimal)size),
                CurrentPage: current,
                Values: result);
        }

        public async Task<Result<Account>> GetAccountDetailIncludeIdentity(AccountId accountId, CancellationToken cancellationToken = default)
        {
            var getAccDetail = await _accountRepository.GetById(accountId);
            if (getAccDetail is null)
            {
                return Result.Fail(new NotFoundError());
            }
            var Identity = await _userManager.FindByIdAsync(getAccDetail.IdentityId);
            getAccDetail.UserIdentity = Identity;
            return getAccDetail;
        }

        public async Task<Result<AuthenticationResultDto>> GoogleHandler(string credential, CancellationToken cancellationToken = default)
        {
            var getAllRoles = await _accountRoleRepository.GetRoles();
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential);
                var externalLoginInfo = new ExternalLoginInfo(
                   new ClaimsPrincipal(
                       new ClaimsIdentity(new List<Claim>
                       {
                        new Claim(ExternalAuthenticationOptions.EXTERNAL_IDENTIFIER_CLAIM_NAME, payload.Subject), // Google's user ID
                        new Claim(ExternalAuthenticationOptions.EXTERNAL_EMAIL_CLAIM_NAME, payload.Email),
                        new Claim(ExternalAuthenticationOptions.EXTERNAL_USERNAME_CLAIM_NAME, payload.Name),
                        new Claim(ExternalAuthenticationOptions.EXTERNAL_PROFILE_IMAGE_CLAIM_NAME, payload.Picture)
                       }, "Google")
                   ),
                   "Google", // Login provider name
                   payload.Subject, // Provider key (Google user ID)
                   "Google" // Email
                );
                var result = await _signinManager.ExternalLoginSignInAsync(
                    externalLoginInfo.LoginProvider,
                    externalLoginInfo.ProviderKey,
                    isPersistent: false
                );
                if (result.Succeeded)
                {
                    var getUserByEmail = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
                    if (await _userManager.IsLockedOutAsync(getUserByEmail) is true)
                        return Result.Fail(AccountErrors.LockAccount);
                    if (getUserByEmail == null)
                        return Result.Fail(new NotFoundError());
                    if (CheckIfUserIsValidToLogin(getUserByEmail))
                    {
                        return Result.Fail(AccountErrors.Login.LoginFail);
                    }
                    var getCustomer = await _accountRepository.GetByIdentityId(getUserByEmail.Id, cancellationToken);
                    if (getCustomer is null)
                        return Result.Fail(new NotFoundError());
                    //getCustomer.Roles.First(r => r.Id != AccountRole.Customer.Id);
                    var authTokenDto = await GenerateTokenForUser(getCustomer.Roles, getCustomer.Email, getCustomer.IdentityId, getCustomer.Id.Value, getCustomer.FullName.Value);
                    return authTokenDto;
                }
                else
                {
                    var getEmail = externalLoginInfo.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_EMAIL_CLAIM_NAME);
                    var getUsername = externalLoginInfo.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_USERNAME_CLAIM_NAME);
                    var getProfileImage = externalLoginInfo.Principal.FindFirstValue(ExternalAuthenticationOptions.EXTERNAL_PROFILE_IMAGE_CLAIM_NAME);
                    ArgumentNullException.ThrowIfNull(getEmail);
                    ArgumentNullException.ThrowIfNull(getUsername);
                    var identity = new CustomIdentityUser()
                    {
                        Email = getEmail,
                        UserName = getEmail,
                        EmailConfirmed = false,
                    };
                    var tryGetByEmail = await _userManager.FindByEmailAsync(getEmail);
                    if(tryGetByEmail is not null)
                    {
                        return Result.Fail(AccountErrors.Register.UserExist);
                    }
                    await _unitOfWork.BeginTransactionAsync();
                    var createResult = await _userManager.CreateAsync(identity);
                    await _userManager.SetLockoutEnabledAsync(identity, false);
                    if (createResult.Succeeded is false)
                        return Result.Fail("fail to create identity");
                    var createLogin = await _userManager.AddLoginAsync(identity, externalLoginInfo);
                    if (createLogin.Succeeded is false)
                        return Result.Fail("fail to create login");
                    FullName fullname;
                    string[] splitedName = getUsername.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (splitedName.Length >= 2)
                    {
                        fullname = FullName.Create(splitedName[0], splitedName[1]);
                    }
                    else
                    {
                        fullname = FullName.Create(splitedName[0], "");
                    }
                    var customer = Account.CreateBaseCustomer(fullname, getEmail, identity.Id, getAllRoles);
                    await _accountRepository.Create(customer);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();
                    var token = await GenerateTokenForUser(customer.Roles, getEmail, identity.Id, customer.Id.Value, fullname.Value);
                    return token;
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Invalid google credential"));
            }
            throw new NotImplementedException();
        }

        public async Task<Result> DeleteByIdentityUser(string identityId)
        {
            var getIdentity = await _userManager.FindByIdAsync(identityId);
            if(getIdentity is null)
            {
                return Result.Fail(new NotFoundError());
            }
            await _userManager.DeleteAsync(getIdentity);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
