using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Identity.Models;
using DiamondShop.Infrastructure.Options;
using FluentResults;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public AuthenticationService(CustomRoleManager roleManager, CustomSigninManager signinManager, CustomUserManager userManager, IUnitOfWork unitOfWork, ILogger<AuthenticationService> logger, IJwtTokenProvider jwtTokenProvider, ICustomerRepository customerRepository, IAccountRoleRepository accountRoleRepository)
        {
            _roleManager = roleManager;
            _signinManager = signinManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtTokenProvider = jwtTokenProvider;
            _customerRepository = customerRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public Task<Result> ConfirmEmail()
        {
            throw new NotImplementedException();
        }
        public Task<Result<string>> GenerateResetPasswordToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AuthenticationResultDto>> ExternalLogin( CancellationToken cancellationToken = default)
        {
            var info = await _signinManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Result.Fail(new NotFoundError("not found user principle"));
            if ((info.Principal.Claims.FirstOrDefault(c => c.Type == "FAILURE") is not null))
                return Result.Fail("Cannot call request to user infor in google , try again later");
            var getUserByEmail = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (getUserByEmail == null)
                return Result.Fail(new NotFoundError());
            var getCustomer = await _customerRepository.GetByIdentityId(getUserByEmail.Id, cancellationToken);
            if (getCustomer is null)
                return Result.Fail(new NotFoundError());
            List<AccountRole> toAccountRole = getCustomer.Roles.Select(r => (AccountRole)r).ToList();
            var authTokenDto = await GenerateTokenForUser(toAccountRole, getCustomer.Email, getCustomer.IdentityId, getCustomer.Id.Value, getCustomer.FullName.Value);
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
                LockoutEnabled = false,
            };
            var createResult = await _userManager.CreateAsync(identity);
            if (createResult.Succeeded is false)
                return Result.Fail("fail to create identity");
            var createLogin = await _userManager.AddLoginAsync(identity, info);
            if (createLogin.Succeeded is false)
                return Result.Fail("fail to create login");
            FullName fullname;
            string[] splitedName = getUsername.Split(" ",StringSplitOptions.RemoveEmptyEntries );
            if(splitedName.Length >=2)
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
            var tryGetUser = await _userManager.FindByEmailAsync(email);
            if (tryGetUser is null)
            {
                return Result.Fail(new NotFoundError("User Not Found"));
            }
            var getCustomer = await _customerRepository.GetByIdentityId(tryGetUser.Id, cancellationToken);
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
            await _userManager.SetRefreshTokenAsync(identityId, refreshTokenResult.refreshToken, refreshTokenResult.expiredDate,  cancellationToken);
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
                return Result.Fail("User Exist");
            }
            try
            {
                var identity = new CustomIdentityUser();
                identity.Email = email;
                identity.UserName = email;
                identity.LockoutEnabled = false;
                identity.EmailConfirmed = emailEnabled;
                var result = await _userManager.CreateAsync(identity, password);
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public Task<Result> ResetPassword()
        {
            throw new NotImplementedException();
        }

        public Task<Result> SendConfirmEmail()
        {
            throw new NotImplementedException();
        }
    }
}
