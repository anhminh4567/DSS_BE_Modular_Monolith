using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Commands.Accept;
using DiamondShop.Application.Usecases.Orders.Commands.AddToDelivery;
using DiamondShop.Application.Usecases.Orders.Commands.Create;
using DiamondShop.Application.Usecases.Orders.Commands.Preparing;
using DiamondShop.Application.Usecases.Orders.Queries.GetUser;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Orders
{
    [Route("api/Order")]
    [ApiController]
    [Authorize]
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
        [HttpPut("Cancel")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CancelOrder([FromRoute] string orderId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CancelOrderCommand(orderId, userId.Value, false));
                if (result.IsSuccess)
                {
                    return Ok("Order cancelled!");
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Reject")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> RejectOrder([FromQuery] string orderId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CancelOrderCommand(orderId, userId.Value, true));
                if (result.IsSuccess)
                {
                    return Ok("Order rejected!");
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Accept")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> AcceptOrder([FromQuery] AcceptOrderCommand acceptOrderCommand)
        {
            var result = await _sender.Send(acceptOrderCommand);
            if (result.IsSuccess)
            {
                return Ok("Order accepted!");
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Preparing")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> PreparingOrder([FromQuery] PreparingOrderCommand preparingOrderCommand)
        {
            var result = await _sender.Send(preparingOrderCommand);
            if (result.IsSuccess)
            {
                return Ok("Order prepared!");
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPut("AddToDelivery")]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> DeliveringOrder([FromQuery] AddOrderToDeliveryCommand addOrderToDeliveryCommand)
        {
            var result = await _sender.Send(addOrderToDeliveryCommand);
            if (result.IsSuccess)
            {
                return Ok("Order added to delivery!");
            }
            else
                return MatchError(result.Errors, ModelState);
        }
    }
}
