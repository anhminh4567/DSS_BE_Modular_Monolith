using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountRoleAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiamondShop.Application.Usecases.Accounts.Queries.GetPaging
{
    public record GetAccountPagingQuery(string[]? roleIds, int current = 0, int size = 10) : IRequest<Result<PagingResponseDto<Account>>>;
    internal class GetAccountPagingQueryHandler : IRequestHandler<GetAccountPagingQuery, Result<PagingResponseDto<Account>>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;

        public GetAccountPagingQueryHandler(IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository)
        {
            _accountRepository = accountRepository;
            _accountRoleRepository = accountRoleRepository;
        }

        public async Task<Result<PagingResponseDto<Account>>> Handle(GetAccountPagingQuery request, CancellationToken cancellationToken)
        {
            var trueCurrent = request.current * request.size;
            List<Account> result = new();
            int total = 0;

            var roleQuery = _accountRoleRepository.GetQuery();
            if (request.roleIds != null && request.roleIds.Count() > 0)
            {
                var parsedRolesId = request.roleIds.Select(x => AccountRoleId.Parse(x)).ToList();
                roleQuery = _accountRoleRepository.QueryFilter(roleQuery, x => parsedRolesId.Contains(x.Id));
            }
            roleQuery = _accountRoleRepository.QueryInclude(roleQuery, x => x.Accounts);
            //roleQuery = roleQuery.SelectMany(x => x.Accounts);
            result = roleQuery.SelectMany(x => x.Accounts).Skip(trueCurrent).Take(request.size).ToList();
            total = roleQuery.SelectMany(x => x.Accounts).Count();
            //result = result1.SelectMany(x => x.Accounts).ToList();

            return Result.Ok(new PagingResponseDto<Account>(
                TotalPage: (int)Math.Ceiling((decimal)total / (decimal)request.size),
                CurrentPage: request.current,
                Values: result));
        }
    }
}
