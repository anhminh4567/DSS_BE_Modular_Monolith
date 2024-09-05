using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Services.Interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Staffs.Commands.Security.Login
{
    public record LoginCommand(string email, string password) :IRequest<Result<AuthenticationResultDto>>;
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResultDto>>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticationResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.LoginStaff(request.email,request.password,cancellationToken);
        }
    }
}
