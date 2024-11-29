using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using DiamondShop.Domain.Services.interfaces;
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
        private readonly IAccountServices _accountServices;
        private const int PAGE_SIZE = 999;

        public AccountManagerWorkers(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IAccountServices accountServices)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
            _optionsMonitor = optionsMonitor;
            _accountServices = accountServices;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //put it there not run, the useer rank is alreeady check when complete order
            //await UpdateUserRank(context);
        }
        private async Task UpdateUserRank(IJobExecutionContext jobContext)
        {
            var getAllUserRoles = (await _accountRoleRepository.GetRoles()).Where(x => x.RoleType == Domain.Models.AccountRoleAggregate.AccountRoleType.Customer);
            var goldRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerGold.Id);
            var silverRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerSilver.Id);
            var bronzeRank = getAllUserRoles.FirstOrDefault(x => x.Id == AccountRole.CustomerBronze.Id);
            var option = _optionsMonitor.CurrentValue.AccountRules;
            var getUserAccount = await _authenticationService.GetAccountPagingIncludeIdentity(new string[] { AccountRole.Customer.Id.Value },null ,0, PAGE_SIZE);
            await _accountServices.CheckAndUpdateUserRankIfQualified(getUserAccount.Values.ToList(), getAllUserRoles.ToList(), option);
            await _unitOfWork.SaveChangesAsync(jobContext.CancellationToken);
        }
    }
}
