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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Accounts.Commands
{
    public record AccountRuleRequestDto(int? MaxAddress, decimal? VndPerPoint, decimal? TotalPointToBronze, decimal? TotalPointToSilver, decimal? TotalPointToGold);
    public record UpdateAccountRuleCommand(AccountRuleRequestDto requestDto): IRequest<Result<AccountRules>>;
    internal class UpdateAccountRuleCommandHandler : IRequestHandler<UpdateAccountRuleCommand, Result<AccountRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<AccountRules> _validator;

        public UpdateAccountRuleCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<AccountRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<AccountRules>> Handle(UpdateAccountRuleCommand request, CancellationToken cancellationToken)
        {

            var accountRuleRequest = request.requestDto;
            var accountRule = _optionsMonitor.CurrentValue.AccountRules;
            var clonedAccountRule = JsonConvert.DeserializeObject<AccountRules>(JsonConvert.SerializeObject(accountRule));
            if (clonedAccountRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            accountRuleRequest.Adapt(clonedAccountRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedAccountRule);
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
                _applicationSettingService.Set(AccountRules.key, clonedAccountRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.AccountRules);
        }
    }
    
}
