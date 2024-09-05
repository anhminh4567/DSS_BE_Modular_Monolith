using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
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

namespace DiamondShop.Application.Usecases.Staffs.Commands.Security.RefreshToken
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Result<AuthenticationResultDto>>;
    internal class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IStaffRepository _staffRepository;

        public RefreshTokenCommandHandler(IAuthenticationService authenticationService, IStaffRepository staffRepository)
        {
            _authenticationService = authenticationService;
            _staffRepository = staffRepository;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
           var getPrincipleResult = await _authenticationService.GetClaimsPrincipalFromCurrentUserContext();
            if (getPrincipleResult.IsFailed)
                return Result.Fail(getPrincipleResult.Errors);
            ClaimsPrincipal claimsPrincipal = getPrincipleResult.Value;
            var userId = claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.USER_ID_CLAIM_NAME)).Value;
            var identityId= claimsPrincipal.Claims.First(claim => claim.Type.Equals(IJwtTokenProvider.IDENTITY_CLAIM_NAME)).Value;
            
            Staff getStaff = await _staffRepository.GetById(cancellationToken, StaffId.Parse(userId));
            if (getStaff is null)
                return Result.Fail(new NotFoundError());

            List<AccountRole> mapedRole = getStaff.Roles.Select(r => (AccountRole)r).ToList();
            var refreshingResult = await _authenticationService.RefreshingToken(request.refreshToken, mapedRole, getStaff.Email, getStaff.IdentityId, getStaff.Id.value, getStaff.FullName.Value, cancellationToken);
            if(refreshingResult.IsSuccess is false)
                return Result.Fail(refreshingResult.Errors);

            AuthenticationResultDto newlyGeneratedToken = refreshingResult.Value;
            return Result.Ok(newlyGeneratedToken);
            //throw new NotImplementedException();
        }
    }
}
