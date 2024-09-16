using DiamondShop.Application.Services.Interfaces;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.BanAccount
{
    public record BanAccountCommand(string identityId) : IRequest<Result>;
    internal class BanAccountCommandHandler : IRequestHandler<BanAccountCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public BanAccountCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(BanAccountCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.BanAccount(request.identityId, cancellationToken);
        }
    }
}
