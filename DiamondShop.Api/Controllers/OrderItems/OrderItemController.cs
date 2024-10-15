using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Queries.GetUser;
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
        [HttpPut("Reject/{orderId}")]
        [Authorize(Roles = AccountRole.StaffId)]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> RejectOrder([FromQuery] string orderItemId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CancelOrderCommand(orderItemId, userId.Value, true));
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
    }
}
