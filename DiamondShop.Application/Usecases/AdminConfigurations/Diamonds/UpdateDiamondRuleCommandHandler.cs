using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShop.Application.Usecases.AdminConfigurations.Diamonds
{
    public record UpdateDiamondRuleCommand(DiamondRuleRequestDto diamondRuleRequestDto) : IRequest<Result<DiamondRule>>;
    internal class UpdateDiamondRuleCommandHandler : IRequestHandler<UpdateDiamondRuleCommand, Result<DiamondRule>>
    {
        private readonly IMapper _mapper;
        private readonly IDiamondRuleConfigurationService _service;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdateDiamondRuleCommandHandler(IMapper mapper, IDiamondRuleConfigurationService service, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _mapper = mapper;
            _service = service;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<DiamondRule>> Handle(UpdateDiamondRuleCommand request, CancellationToken cancellationToken)
        {
            var diamondruleRequest = request.diamondRuleRequestDto;
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            var clonedDiamondRule = JsonConvert.DeserializeObject<DiamondRule>(JsonConvert.SerializeObject(diamondRule));
            if (clonedDiamondRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            diamondruleRequest.Adapt(clonedDiamondRule);
            var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }
            var getDiamondReul = await _service.GetConfiguration();
            return Result.Ok(getDiamondReul);
            throw new NotImplementedException();
        }
    }
}
