using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.DiamondRuleConfig.Models;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig;
using DiamondShop.Application.Services.Interfaces.AdminConfigurations.PromotionRuleConfig.Models;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions
{
    public record UpdatePromotionRuleCommand(PromotionRuleRequestDto promotionRuleRequest) : IRequest<Result<PromotionRule>>;
    internal class UpdatePromotionRuleCommandHandler : IRequestHandler<UpdatePromotionRuleCommand, Result<PromotionRule>>
    {
        private readonly IMapper _mapper;
        private readonly IPromotionRuleConfigurationService _service;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;

        public UpdatePromotionRuleCommandHandler(IMapper mapper, IPromotionRuleConfigurationService service, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor)
        {
            _mapper = mapper;
            _service = service;
            _optionsMonitor = optionsMonitor;
        }

        public async Task<Result<PromotionRule>> Handle(UpdatePromotionRuleCommand request, CancellationToken cancellationToken)
        {
            var promotionruleRequest = request.promotionRuleRequest;
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            var clonedPromotionRule = JsonConvert.DeserializeObject<PromotionRule>(JsonConvert.SerializeObject(promotionRule));
            if (clonedPromotionRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            promotionruleRequest.Adapt(clonedPromotionRule);
            var updateResult = await _service.SetConfiguration(clonedPromotionRule);
            if (updateResult.IsFailed)
            {
                return Result.Fail(updateResult.Errors);
            }
            var getPromotionRule = await _service.GetConfiguration();
            return Result.Ok(getPromotionRule);
            throw new NotImplementedException();
        }
    }
}
