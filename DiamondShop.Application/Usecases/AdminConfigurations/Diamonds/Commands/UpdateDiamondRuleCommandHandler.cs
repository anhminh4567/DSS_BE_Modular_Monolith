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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Commands
{
    public record UpdateDiamondRuleCommand(DiamondRuleRequestDto diamondRuleRequestDto) : IRequest<Result<DiamondRule>>;
    internal class UpdateDiamondRuleCommandHandler : IRequestHandler<UpdateDiamondRuleCommand, Result<DiamondRule>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondRule> _validator;

        public UpdateDiamondRuleCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<DiamondRule> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<DiamondRule>> Handle(UpdateDiamondRuleCommand request, CancellationToken cancellationToken)
        {
            var diamondruleRequest = request.diamondRuleRequestDto;
            var diamondRule = _optionsMonitor.CurrentValue.DiamondRule;
            var clonedDiamondRule = JsonConvert.DeserializeObject<DiamondRule>(JsonConvert.SerializeObject(diamondRule));
            if (clonedDiamondRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            diamondruleRequest.Adapt(clonedDiamondRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedDiamondRule);
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
                _applicationSettingService.Set(DiamondRule.key, clonedDiamondRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.DiamondRule);
            //if (updateResult.IsFailed)
            //{
            //    return Result.Fail(updateResult.Errors);
            //}
            //var getDiamondReul = await _service.GetConfiguration();
            //return Result.Ok(getDiamondReul);
            //throw new NotImplementedException();
        }
    }
}
