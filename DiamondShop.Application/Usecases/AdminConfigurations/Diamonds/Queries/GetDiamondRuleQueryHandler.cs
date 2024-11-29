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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Queries
{
    public record GetDiamondRuleQuery() : IRequest<Result<DiamondRule>>;
    internal class GetDiamondRuleQueryHandler : IRequestHandler<GetDiamondRuleQuery, Result<DiamondRule>>
    {
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondRule> _validator;

        public GetDiamondRuleQueryHandler(IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<DiamondRule> validator)
        {
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<DiamondRule>> Handle(GetDiamondRuleQuery request, CancellationToken cancellationToken)
        {
            var get = _optionsMonitor.CurrentValue.DiamondRule;
            return Result.Ok(get);
            throw new NotImplementedException();
        }
    }
}
