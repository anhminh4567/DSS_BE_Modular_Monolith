using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Application.Usecases.Orders.Queries.GetUser;
using DiamondShop.Domain.Models.AccountAggregate.ValueObjects;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Orders
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public OrderController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("User/All")]
        [Produces<List<OrderDto>>]
        public async Task<ActionResult> GetUserOrder([FromQuery] string accountId)
        {
            var result = await _sender.Send(new GetUserOrderQuery(accountId));
            var mappedResult = _mapper.Map<List<OrderDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost("Checkout")]
        public async Task<ActionResult> Checkout(CreateOrderCommand orderCreateCommand)
        {
            var result = await _sender.Send(orderCreateCommand);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Cancel/{orderId}")]
        public async Task<ActionResult> CancelOrder([FromQuery] string orderId)
        {
            var result = await _sender.Send(new CancelOrderCommand(orderId));
            var mappedResult = _mapper.Map<OrderDto>(result);
            return Ok(mappedResult);
        }
    }
}
