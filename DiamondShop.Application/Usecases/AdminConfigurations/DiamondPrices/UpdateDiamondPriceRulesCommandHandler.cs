using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.AdminConfigurations.Accounts.Commands;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.DiamondPrices
{
    public record DiamondPriceRulesRequestDto(string? DefaultRoundCriteriaPriceBoard, string? DefaultFancyCriteriaPriceBoard, string? DefaultSideDiamondCriteriaPriceBoard);
    public record UpdateDiamondPriceRulesCommand(DiamondPriceRulesRequestDto requestDto) : IRequest<Result<DiamondPriceRules>>;
    internal class UpdateDiamondPriceRulesCommandHandler : IRequestHandler<UpdateDiamondPriceRulesCommand, Result<DiamondPriceRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<DiamondPriceRules> _validator;

        public UpdateDiamondPriceRulesCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<DiamondPriceRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<DiamondPriceRules>> Handle(UpdateDiamondPriceRulesCommand request, CancellationToken cancellationToken)
        {
            var diamondPriceRuleRequest = request.requestDto;
            var diamondPriceRule = _optionsMonitor.CurrentValue.DiamondPriceRules;
            var clonedDiamondPriceRule = JsonConvert.DeserializeObject<DiamondPriceRules>(JsonConvert.SerializeObject(diamondPriceRule));
            if (clonedDiamondPriceRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            diamondPriceRuleRequest.Adapt(clonedDiamondPriceRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedDiamondPriceRule);
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
                _applicationSettingService.Set(DiamondRule.key, clonedDiamondPriceRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.DiamondPriceRules);
            throw new NotImplementedException();
        }
    }
}
