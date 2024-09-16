using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Queries.GetDetail
{
    public record GetAccountDetailQuery(AccountId? AccountId = null) : IRequest<Result<Account>>;
    internal class GetAccountDetailQueryHandler : IRequestHandler<GetAccountDetailQuery, Result<Account>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetAccountDetailQueryHandler(IAccountRepository accountRepository, IHttpContextAccessor contextAccessor)
        {
            _accountRepository = accountRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<Account>> Handle(GetAccountDetailQuery request, CancellationToken cancellationToken)
        {
            Account? getAccDetail;
            if (request.AccountId is null)
            {
                HttpContext? httpContext = _contextAccessor.HttpContext;
                if (httpContext == null)
                    throw new Exception("http context not found, unknown reasons");
                var claims = httpContext.User.Claims.ToList();
                Claim? getIdentityClaim = claims.FirstOrDefault(c => c.Type == IJwtTokenProvider.IDENTITY_CLAIM_NAME);
                if (getIdentityClaim == null)
                    return Result.Fail(new NotFoundError("not found identity of this user"));
                getAccDetail = await _accountRepository.GetByIdentityId(getIdentityClaim.Value, cancellationToken);
                if (getAccDetail == null)
                    return Result.Fail(new NotFoundError("noit found user with this identity"));
            }
            else
            {
                getAccDetail = await _accountRepository.GetById(request.AccountId);
            }
            return Result.Ok(getAccDetail);
        }
    }

}
