using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Infrastructure.Databases;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    internal class AccountManagerWorkers : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private const int PAGE_SIZE = 999;

        public AccountManagerWorkers(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _optionsMonitor = optionsMonitor;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await UpdateUserRank(context);
        }
        private async Task UpdateUserRank(IJobExecutionContext jobContext)
        {
            var getAllUserRoles = (await _accountRoleRepository.GetRoles()).Where(x => x.RoleType == Domain.Models.AccountRoleAggregate.AccountRoleType.Customer);
            var goldRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerGold.Id);
            var silverRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerSilver.Id);
            var bronzeRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerBronze.Id);
            var option = _optionsMonitor.CurrentValue.AccountRules;
            var getUserAccount = await _authenticationService.GetAccountPagingIncludeIdentity(new string[] { AccountRole.Customer.Id.Value }, 0, PAGE_SIZE);
            foreach (var acc in getUserAccount.Values)
            {
                var userRoles = acc.Roles;
                var userPoint = acc.TotalPoint;
                if (IsGoldRankQualified(acc, option, userRoles))
                    userRoles.Add(goldRank);

                if (IsSilverRankQualified(acc, option, userRoles))
                    userRoles.Add(silverRank);

                if(IsBronzeRankQualified(acc, option, userRoles))
                    userRoles.Add(bronzeRank);
            }
            await _unitOfWork.SaveChangesAsync(jobContext.CancellationToken);
        }
        private bool IsGoldRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerGold.Id)) // already in rank
                return false;
            var goldRank = options.TotalPointToGold;
            if (user.TotalPoint >= goldRank)
                return true;
            return false;
        }
        private bool IsSilverRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerSilver.Id)) // already in rank
                return false;
            var silverRank = options.TotalPointToSilver;
            if (user.TotalPoint >= silverRank)
                return true;
            return false;
        }
        private bool IsBronzeRankQualified(Account user, AccountRules options, List<AccountRole> roles)
        {
            if (roles.Any(x => x.Id == AccountRole.CustomerBronze.Id)) // already in rank
                return false;
            var bronzeRank = options.TotalPointToBronze;
            if (user.TotalPoint >= bronzeRank)
                return true;
            return false;
        }

    }
}
