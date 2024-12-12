using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.Accounts.Commands.BanAccount
{
    public record BanAccountCommand(string identityId, string? banEndDate) : IRequest<Result>;
    internal class BanAccountCommandHandler : IRequestHandler<BanAccountCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public BanAccountCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(BanAccountCommand request, CancellationToken cancellationToken)
        {
            DateTime? endDateTimeUtc = null;
            if (request.banEndDate != null)
            {
                bool parseSuccess = DateTime.TryParseExact(request.banEndDate, DateTimeFormatingRules.DateTimeFormat,null, DateTimeStyles.None, out var result);
                if (parseSuccess)
                    endDateTimeUtc = result.ToUniversalTime();
                else
                    endDateTimeUtc = null;
            }
            var banResult =  await _authenticationService.BanAccount(request.identityId, endDateTimeUtc, cancellationToken);
            return banResult;
        }
    }
}
