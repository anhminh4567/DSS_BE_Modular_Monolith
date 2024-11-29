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

namespace DiamondShop.Application.Usecases.AdminConfigurations.CartModels.Commands
{
    public record UpdateCartModelRuleCommand(CartModelRules requestDto) : IRequest<Result<CartModelRules>>;
    internal class UpdateCartModelRuleCommandHandler : IRequestHandler<UpdateCartModelRuleCommand, Result<CartModelRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<CartModelRules> _validator;

        public UpdateCartModelRuleCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<CartModelRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<CartModelRules>> Handle(UpdateCartModelRuleCommand request, CancellationToken cancellationToken)
        {
            var cartModelRuleRequest = request.requestDto;
            var cartModelRule = _optionsMonitor.CurrentValue.DiamondRule;
            var clonedcartModelRule = JsonConvert.DeserializeObject<CartModelRules>(JsonConvert.SerializeObject(cartModelRule));
            if (clonedcartModelRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            cartModelRuleRequest.Adapt(clonedcartModelRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedcartModelRule);
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
                _applicationSettingService.Set(CartModelRules.key, clonedcartModelRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.CartModelRules);
        }
    }

}
