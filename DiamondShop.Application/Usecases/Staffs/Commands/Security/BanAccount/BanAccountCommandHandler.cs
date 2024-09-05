using DiamondShop.Application.Services.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Staffs.Commands.Security.BanAccount
{
    public record BanAccountCommand(string identityId, bool isBanning = true) : IRequest<Result>;
    internal class BanAccountCommandHandler : IRequestHandler<BanAccountCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public BanAccountCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(BanAccountCommand request, CancellationToken cancellationToken)
        {
            
            return Result.Ok();
        }
    }
}
