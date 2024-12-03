using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds;
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

namespace DiamondShop.Application.Usecases.AdminConfigurations.Locations.Commands
{
    public record LocationRulesRequestDto(string? OriginalLocationName, string? OrignalRoad, string? OrignalWard, string? OrignalDistrict, string? OriginalProvince, string? OrinalPlaceId, string? OriginalLongLat);
    public record UpdateLocationRulesCommand(LocationRulesRequestDto diamondRuleRequestDto) : IRequest<Result<LocationRules>>;
    internal class UpdateLocationRulesCommandHandler : IRequestHandler<UpdateLocationRulesCommand, Result<LocationRules>>
    {
        private readonly IMapper _mapper;
        private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IValidator<LocationRules> _validator;

        public UpdateLocationRulesCommandHandler(IMapper mapper, IOptionsMonitor<ApplicationSettingGlobal> optionsMonitor, IApplicationSettingService applicationSettingService, IValidator<LocationRules> validator)
        {
            _mapper = mapper;
            _optionsMonitor = optionsMonitor;
            _applicationSettingService = applicationSettingService;
            _validator = validator;
        }

        public async Task<Result<LocationRules>> Handle(UpdateLocationRulesCommand request, CancellationToken cancellationToken)
        {
            var locationRuleRequest = request.diamondRuleRequestDto;
            var locationRule = _optionsMonitor.CurrentValue.LocationRules;
            var clonedlocationRule = JsonConvert.DeserializeObject<LocationRules>(JsonConvert.SerializeObject(locationRule));
            if (clonedlocationRule is null)
                return Result.Fail("Không thể clone diamond rule cũ được");
            locationRuleRequest.Adapt(clonedlocationRule);
            //var updateResult = await _service.SetConfiguration(clonedDiamondRule);
            var validationResult = _validator.Validate(clonedlocationRule);
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
                _applicationSettingService.Set(LocationRules.Key, clonedlocationRule);
            }
            return Result.Ok(_optionsMonitor.CurrentValue.LocationRules);
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
