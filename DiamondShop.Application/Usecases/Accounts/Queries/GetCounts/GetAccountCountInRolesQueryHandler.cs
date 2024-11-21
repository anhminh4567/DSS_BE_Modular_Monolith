using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Queries.GetCounts
{
    public record GetAccountCountInRolesQuery (List<string> roles) : IRequest<int>;
    internal class GetAccountCountInRolesQueryHandler : IRequestHandler<GetAccountCountInRolesQuery, int>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public GetAccountCountInRolesQueryHandler(IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<int> Handle(GetAccountCountInRolesQuery request, System.Threading.CancellationToken cancellationToken)
        {
            var parsedRoleIds = request.roles.Select(x => AccountRoleId.Parse(x)).ToList();
            var allRoles = await _accountRoleRepository.GetAll();
            var selectedRoles = allRoles.Where(r => parsedRoleIds.Contains(r.Id)).ToList();
            var count = await _accountRepository.GetAccountCountsInRoles(selectedRoles, cancellationToken);
            return count;
        }
    }
    
}
