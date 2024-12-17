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

namespace DiamondShop.Application.Usecases.AdminConfigurations.BankAccounts.Commands
{
    public record ShopBankAccountRulesRequestDto(string? AccountNumber, string? AccountName, string? BankBin, string? BankName);

    public record UpdateShopBankAccountRuleCommand(ShopBankAccountRulesRequestDto RequestDto): IRequest<Result<ShopBankAccountRules>>;
    internal class UpdateShopBankAccountRuleCommandHandler : IRequestHandler<UpdateShopBankAccountRuleCommand, Result<ShopBankAccountRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<ShopBankAccountRules> _validator;

        public UpdateShopBankAccountRuleCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<ShopBankAccountRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<ShopBankAccountRules>> Handle(UpdateShopBankAccountRuleCommand request, CancellationToken cancellationToken)
        {
            var shopBankAccountRuleRequest = request.RequestDto;
            var shopBankAccountlRule = _optionsMonitor.CurrentValue.ShopBankAccountRules;
            var clonedShopBankAccountRule = JsonConvert.DeserializeObject<ShopBankAccountRules>(JsonConvert.SerializeObject(shopBankAccountlRule));
            if (clonedShopBankAccountRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            shopBankAccountRuleRequest.Adapt(clonedShopBankAccountRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedShopBankAccountRule);
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
                _applicationSettingService.Set(ShopBankAccountRules.Key, clonedShopBankAccountRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.ShopBankAccountRules);
        }
    }
}
