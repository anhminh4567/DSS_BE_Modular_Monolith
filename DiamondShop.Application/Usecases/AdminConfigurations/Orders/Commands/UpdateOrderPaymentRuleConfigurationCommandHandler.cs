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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders.Commands
{
    public record OrderPaymentRuleRequestDto(int? DepositPercent,int? CODPercent, int? PayAllFine, decimal? MaxMoneyFine, decimal? MinAmountForCOD, int? CODHourTimeLimit, List<string> LockedPaymentMethodOnCustomer);
    public record UpdateOrderPaymentRuleConfigurationCommand(OrderPaymentRuleRequestDto requestDto) : IRequest<Result<OrderPaymentRules>>;

    internal class UpdateOrderPaymentRuleConfigurationCommandHandler : IRequestHandler<UpdateOrderPaymentRuleConfigurationCommand, Result<OrderPaymentRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<OrderPaymentRules> _validator;

        public UpdateOrderPaymentRuleConfigurationCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<OrderPaymentRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<OrderPaymentRules>> Handle(UpdateOrderPaymentRuleConfigurationCommand request, CancellationToken cancellationToken)
        {
            var orderPaymentRuleRequest = request.requestDto;
            var orderPaymentRule = _optionsMonitor.CurrentValue.OrderPaymentRules;
            var clonedorderPaymentRule = JsonConvert.DeserializeObject<OrderPaymentRules>(JsonConvert.SerializeObject(orderPaymentRule));
            if (clonedorderPaymentRule is null)
                return Result.Fail("Không thể clone order payment rule cũ được");
            orderPaymentRuleRequest.Adapt(clonedorderPaymentRule);
            //var updateResult = await _orderRuleConfigurationServices.SetConfiguration(clonedOrderRule);
            var validationResult = _validator.Validate(clonedorderPaymentRule);
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
                _applicationSettingService.Set(OrderPaymentRules.key, clonedorderPaymentRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.OrderPaymentRules);
        }
    }
}
