using DiamondShop.Application.Commons.Responses;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiamondShop.Application.Usecases.Accounts.Queries.GetPaging
{
    public record GetAccountPagingQuery(int current = 0, int size = 10) : IRequest<Result<PagingResponseDto<Account>>>;
    internal class GetAccountPagingQueryHandler : IRequestHandler<GetAccountPagingQuery, Result<PagingResponseDto<Account>>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccountPagingQueryHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Result<PagingResponseDto<Account>>> Handle(GetAccountPagingQuery request, CancellationToken cancellationToken)
        {
            var query = _accountRepository.GetQuery();
            var trueCurrent = request.current * request.size;
            query = query.Skip(trueCurrent).Take(request.size);

            var result = query.ToList();
            var totalPage = _accountRepository.GetCount();
            return Result.Ok(new PagingResponseDto<Account>(
                totalPage: totalPage,
                currentPage: request.current,
                Values: result));
        }
    }
}
