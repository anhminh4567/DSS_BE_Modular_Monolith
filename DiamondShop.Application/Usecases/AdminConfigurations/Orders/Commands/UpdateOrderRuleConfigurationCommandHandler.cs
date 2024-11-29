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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Orders.Commands
{
    public record UpdateOrderRuleConfigurationCommand(OrderRule requestDto) : IRequest<Result<OrderRule>>;
    internal class UpdateOrderRuleConfigurationCommandHandler : IRequestHandler<UpdateOrderRuleConfigurationCommand, Result<OrderRule>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<OrderRule> _validator;

        public UpdateOrderRuleConfigurationCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<OrderRule> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<OrderRule>> Handle(UpdateOrderRuleConfigurationCommand request, CancellationToken cancellationToken)
        {
            var orderRuleRequest = request.requestDto;
            var orderRule = _optionsMonitor.CurrentValue.OrderRule;
            var clonedOrderRule = JsonConvert.DeserializeObject<OrderRule>(JsonConvert.SerializeObject(orderRule));
            if (clonedOrderRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            orderRuleRequest.Adapt(clonedOrderRule);
            //var updateResult = await _orderRuleConfigurationServices.SetConfiguration(clonedOrderRule);
            var validationResult = _validator.Validate(clonedOrderRule);
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
                _applicationSettingService.Set(OrderRule.key, clonedOrderRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.OrderRule);
            //if (updateResult.IsFailed)
            //{
            //    return Result.Fail(updateResult.Errors);
            //}
            //var getRule = await _orderRuleConfigurationServices.GetConfiguration();
            //return Result.Ok(getRule);
            //throw new NotImplementedException();
        }
    }

}
