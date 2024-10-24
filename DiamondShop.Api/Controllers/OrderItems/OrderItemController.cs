using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.OrderItems.Command.Cancel;
using DiamondShop.Application.Usecases.OrderItems.Command.Prepare;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.OrderItems
{
    [Route("api/OrderItem")]
    [ApiController]
    [Authorize]
    public class OrderItemController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public OrderItemController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpPut("Prepare")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> PrepareOrderItem([FromQuery] PrepareOrderItemCommand prepareOrderItemCommand)
        {
            var result = await _sender.Send(prepareOrderItemCommand);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPut()]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> CancelOrderItem([FromQuery] List<string> orderItemIds)
        {
            var result = await _sender.Send(new CancelOrderItemCommand(orderItemIds));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
    }
}
