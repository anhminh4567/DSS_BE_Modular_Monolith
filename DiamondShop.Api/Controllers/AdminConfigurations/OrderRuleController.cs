using DiamondShop.Application.Usecases.AdminConfigurations.Frontends;
using DiamondShop.Application.Usecases.AdminConfigurations.Orders;
using DiamondShop.Application.Usecases.AdminConfigurations.Orders.Commands;
using DiamondShop.Application.Usecases.AdminConfigurations.Orders.Queries;
using DiamondShop.Domain.BusinessRules;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.AdminConfigurations
{
    [Route("api/Configuration/OrderRule")]
    [Tags("Configuration")]
    [ApiController]
    public class OrderRuleController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public OrderRuleController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult> GetConfiguration()
        {
            var get = await _sender.Send(new GetOrderRuleConfigurationQuery());
            return Ok(get.Value);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateConfiguration([FromBody] OrderRule command)
        {
            var updateResul = await _sender.Send(new UpdateOrderRuleConfigurationCommand(command));
            if (updateResul.IsFailed)
            {
                return MatchError(updateResul.Errors, ModelState);
            }
            return Ok(updateResul.Value);
        }
    }
}
