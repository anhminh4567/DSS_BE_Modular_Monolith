using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Locations.Queries
{
    public record GetLocationRuleQuery() : IRequest<Result<LocationRules>>;
    internal class GetLocationRulesQueryHandler : IRequestHandler<GetLocationRuleQuery, Result<LocationRules>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondRule> _validator;

        public GetLocationRulesQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<DiamondRule> validator)
        {
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<LocationRules>> Handle(GetLocationRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.LocationRules;
            return Result.Ok(get);
            throw new NotImplementedException();
        }
    }
}
