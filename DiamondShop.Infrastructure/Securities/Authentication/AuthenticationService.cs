using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using DiamondShop.Infrastructure.Identity.Models;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Securities.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly CustomRoleManager _roleManager;
        private readonly CustomSigninManager _signinManager;
        private readonly CustomUserManager _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IJwtTokenProvider _jwtTokenProvider;

        public AuthenticationService(CustomRoleManager roleManager, CustomSigninManager signinManager, CustomUserManager userManager, IUnitOfWork unitOfWork, ILogger<AuthenticationService> logger, IJwtTokenProvider jwtTokenProvider)
        {
            _roleManager = roleManager;
            _signinManager = signinManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public Task<Result> ConfirmEmail()
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> GenerateResetPasswordToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<IUserIdentity>> GetUserIdentity(string identityId, CancellationToken cancellationToken = default)
        {
            var findResult = await _userManager.FindByIdAsync(identityId);
            if(findResult is null)
            {
                return Result.Fail(new NotFoundError("User Not Found"));
            }
            return findResult;
        }

        public async Task<Result<AuthenticationResultDto>> Login(string email, string password, CancellationToken cancellationToken = default)
        {
            var tryGetUser = await _userManager.FindByEmailAsync(email);
            if (tryGetUser is null)
            {
                return Result.Fail(new NotFoundError("User Not Found"));
            }
            try
            {
                var accTokenResult = _jwtTokenProvider.GenerateAccessToken(tryGetUser);
                var refreshTokenResult = _jwtTokenProvider.GenerateRefreshToken(tryGetUser);

                return Result.Ok(new AuthenticationResultDto(
                    accessToken: accTokenResult.accessToken,
                    expiredAccess: accTokenResult.expiredDate,
                    refreshToken: refreshTokenResult.refreshToken,
                    expiredRefresh: accTokenResult.expiredDate
                ));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Result> Logout(IUserIdentity userIdentity, CancellationToken cancellationToken = default)
        {
            await _userManager.RemoveRefreshTokenAsync(userIdentity,cancellationToken);
            return Result.Ok();
        }

        public async Task<Result<string>> Register(string email, string password, FullName fullname, CancellationToken cancellationToken = default)
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
                var result = await _userManager.CreateAsync(identity, password);
                if (result.Succeeded is false)
                {
                    var errDict = new Dictionary<string, object>();
                    foreach(var err in result.Errors)
                    {
                        errDict.Add(err.Code, err.Description);
                    }
                    return Result.Fail(new ValidationError("Password Error", errDict));
                }
                return Result.Ok(identity.IdentityId);

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
