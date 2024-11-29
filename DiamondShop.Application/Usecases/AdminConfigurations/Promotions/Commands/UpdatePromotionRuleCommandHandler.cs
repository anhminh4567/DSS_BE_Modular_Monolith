using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Commons;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Promotions.Commands
{
    public record UpdatePromotionRuleCommand(PromotionRuleRequestDto promotionRuleRequest) : IRequest<Result<PromotionRule>>;
    internal class UpdatePromotionRuleCommandHandler : IRequestHandler<UpdatePromotionRuleCommand, Result<PromotionRule>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<PromotionRule> _validator;

        public UpdatePromotionRuleCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<PromotionRule> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<PromotionRule>> Handle(UpdatePromotionRuleCommand request, CancellationToken cancellationToken)
        {
            var promotionruleRequest = request.promotionRuleRequest;
            var promotionRule = _optionsMonitor.CurrentValue.PromotionRule;
            var clonedPromotionRule = JsonConvert.DeserializeObject<PromotionRule>(JsonConvert.SerializeObject(promotionRule));
            if (clonedPromotionRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            promotionruleRequest.Adapt(clonedPromotionRule);

            //var updateResult = await _service.SetConfiguration(clonedPromotionRule);
            var validationResult = _validator.Validate(clonedPromotionRule);
            if (validationResult.IsValid is false)
            {
                Dictionary<string, object> validationErrors = new();
                validationResult.Errors
                    .ForEach(input =>
                    {
                        if (validationErrors.ContainsKey(input.PropertyName))
                        {
                            var errorList = (List<object>)validationErrors[input.PropertyName];
                            errorList.Add(input.ErrorMessage);
                        }
                        else
                            validationErrors.Add(input.PropertyName, new List<object> { input.ErrorMessage });
                    });
                ValidationError validationError = new ValidationError($"validation error ", validationErrors);
                return Result.Fail(validationError);
            }
            else
            {
                _applicationSettingService.Set(PromotionRule.key, clonedPromotionRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.PromotionRule);
            //if (updateResult.IsFailed)
            //{
            //    return Result.Fail(updateResult.Errors);
            //}
            //var getPromotionRule = await _service.GetConfiguration();
            //return Result.Ok(getPromotionRule);
            //throw new NotImplementedException();
        }
    }
}
