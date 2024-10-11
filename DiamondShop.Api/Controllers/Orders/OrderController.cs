using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Usecases.Orders.Queries.GetUser;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DiamondShop.Api.Controllers.Orders
{
    [Route("api/Order/All")]
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
        [HttpGet("User")]
        [Produces<List<OrderDto>>]
        public async Task<ActionResult> GetUserOrder([FromQuery] string accountId)
        {
            var result = await _sender.Send(new GetUserOrderQuery(accountId));
            var mappedResult = _mapper.Map<List<OrderDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost]
        public async Task<ActionResult> Checkout()
        {
            throw new NotImplementedException();
        }

    }
}
