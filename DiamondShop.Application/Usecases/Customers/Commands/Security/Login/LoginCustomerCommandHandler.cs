using BeatvisionRemake.Application.Services.Interfaces;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Data;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.Repositories;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Customers.Commands.Security.Login
{
    public record LoginCustomerCommand(string? email, string? password, bool isExternalLogin = false) : IRequest<Result<AuthenticationResultDto>>;
    internal class LoginCustomerCommandHandler : IRequestHandler<LoginCustomerCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCustomerCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
        {
            if (request.isExternalLogin is false)
            {
                return await _authenticationService.Login(request.email, request.password, cancellationToken);
            }
            else
            {
                return await _authenticationService.ExternalLogin(cancellationToken);
            }

        }
    }
}
