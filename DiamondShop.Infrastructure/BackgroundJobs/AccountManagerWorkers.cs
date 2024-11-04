using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories;
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

        public AccountManagerWorkers(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
