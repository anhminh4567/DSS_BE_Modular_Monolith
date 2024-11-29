using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Diamonds.Queries;
using DiamondShop.Commons;
using DiamondShop.Domain.BusinessRules;
using DiamondShop.Domain.Common;
using FluentResults;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/DiamondRule")]
    [Tags("Configuration")]
    [ApiController]
    public class DiamondRuleConfigurationController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        //private readonly IOptionsMonitor<ApplicationSettingGlobal> _optionsMonitor;
        //private readonly IApplicationSettingService _applicationSettingService;
        //private readonly IValidator<DiamondRule> _validator;

        public DiamondRuleConfigurationController(ISender sender,IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetDiamondRule()
        {
            var diamondRule = await _sender.Send(new GetDiamondRuleQuery());
            return Ok(diamondRule.Value);
        }
        [HttpPost()]
        public async Task<ActionResult> UpdateDiamondRule([FromForm] DiamondRuleRequestDto diamondRuleRequestDto)
        {
            var updateResul = await _sender.Send(new UpdateDiamondRuleCommand(diamondRuleRequestDto));
            if(updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
