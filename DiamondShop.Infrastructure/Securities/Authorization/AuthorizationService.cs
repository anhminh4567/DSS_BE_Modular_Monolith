using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Common.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.Securities.Authorization
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly CustomRoleManager _roleManager;
        private readonly CustomSigninManager _signinManager;
        private readonly CustomUserManager _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthorizationService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthorizationService(CustomRoleManager roleManager, CustomSigninManager signinManager, CustomUserManager userManager, IUnitOfWork unitOfWork, ILogger<AuthorizationService> logger, IDateTimeProvider dateTimeProvider)
        {
            _roleManager = roleManager;
            _signinManager = signinManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task AddToRole(IUserIdentity identity, Role role, CancellationToken cancellationToken = default)
        {
            var getUser = await _userManager.FindByIdAsync(identity.IdentityId);
            var result = await _userManager.AddToRoleAsync(getUser, role.Name);
            if (result.Succeeded is false)
                throw new Exception("Fail to add Row");
        }

        public async Task Ban(IUserIdentity identity, TimeSpan time, CancellationToken cancellationToken = default)
        {
            var getUser = await _userManager.FindByIdAsync(identity.IdentityId);
            try 
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                await _userManager.SetLockoutEnabledAsync(getUser, true);
                await _userManager.SetLockoutEndDateAsync(getUser, _dateTimeProvider.UtcNow.Add(time));
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex) 
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task RemoveFromRole(IUserIdentity identity, Role role, CancellationToken cancellationToken = default)
        {
            var getUser = await _userManager.FindByIdAsync(identity.IdentityId);
            await _userManager.RemoveFromRoleAsync(getUser, role.Name);
        }
    }
}
