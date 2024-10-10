using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.AccountAggregate;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.RefreshingToken
{
    public record RefreshingTokenCommand(string refreshToken) : IRequest<Result<AuthenticationResultDto>>;
    internal class RefreshingTokenCommandHandler : IRequestHandler<RefreshingTokenCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountRepository _accountRepository;

        public RefreshingTokenCommandHandler(IAuthenticationService authenticationService, IAccountRepository accountRepository)
        {
            _authenticationService = authenticationService;
            _accountRepository = accountRepository;
        }


        public async Task<Result<AuthenticationResultDto>> Handle(RefreshingTokenCommand request, CancellationToken cancellationToken)
        {
            var getPrincipleResult = await _authenticationService.GetClaimsPrincipalFromCurrentUserContext();
            if (getPrincipleResult.IsFailed)
                return Result.Fail(getPrincipleResult.Errors);
            ClaimsPrincipal claimsPrincipal = getPrincipleResult.Value;
            var userId = claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.USER_ID_CLAIM_NAME)).Value;
            var identityId = claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.IDENTITY_CLAIM_NAME)).Value;

            Account getAccount = await _accountRepository.GetById(AccountId.Parse(userId));
            if (getAccount is null)
                return Result.Fail(new NotFoundError());

            var refreshingResult = await _authenticationService.RefreshingToken(request.refreshToken, getAccount.Roles, getAccount.Email, getAccount.IdentityId, getAccount.Id.Value, getAccount.FullName.Value, cancellationToken);
            if (refreshingResult.IsSuccess is false)
                return Result.Fail(refreshingResult.Errors);

            AuthenticationResultDto newlyGeneratedToken = refreshingResult.Value;
            return Result.Ok(newlyGeneratedToken);
            //throw new NotImplementedException();
        }
    }
}
