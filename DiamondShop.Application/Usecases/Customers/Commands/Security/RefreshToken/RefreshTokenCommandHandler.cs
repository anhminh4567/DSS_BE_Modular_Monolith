using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
using DiamondShop.Domain.Models.CustomerAggregate;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Models.RoleAggregate;
using DiamondShop.Domain.Models.StaffAggregate;
using DiamondShop.Domain.Models.StaffAggregate.ValueObjects;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Commands.Security.RefreshToken
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Result<AuthenticationResultDto>>;
    internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerRepository _customerRepository;

        public RefreshTokenCommandHandler(IAuthenticationService authenticationService, ICustomerRepository customerRepository)
        {
            _authenticationService = authenticationService;
            _customerRepository = customerRepository;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var getPrincipleResult = await _authenticationService.GetClaimsPrincipalFromCurrentUserContext();
            if (getPrincipleResult.IsFailed)
                return Result.Fail(getPrincipleResult.Errors);
            ClaimsPrincipal claimsPrincipal = getPrincipleResult.Value;
            var userId = claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.USER_ID_CLAIM_NAME)).Value;
            var identityId = claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.IDENTITY_CLAIM_NAME)).Value;

            Customer getCustomer = await _customerRepository.GetById(cancellationToken, CustomerId.Parse(userId));
            if (getCustomer is null)
                return Result.Fail(new NotFoundError());

            List<AccountRole> mapedRole = getCustomer.Roles.Select(r => (AccountRole)r).ToList();
            var refreshingResult = await _authenticationService.RefreshingToken(request.refreshToken, mapedRole, getCustomer.Email, getCustomer.IdentityId, getCustomer.Id.Value, getCustomer.FullName.Value, cancellationToken);
            if (refreshingResult.IsSuccess is false)
                return Result.Fail(refreshingResult.Errors);

            AuthenticationResultDto newlyGeneratedToken = refreshingResult.Value;
            return Result.Ok(newlyGeneratedToken);
            //throw new NotImplementedException();
        }
    }
}
