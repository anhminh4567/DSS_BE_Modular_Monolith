using DiamondShop.Application.Dtos.Responses.Carts;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.Carts.Commands.Add;
using DiamondShop.Application.Usecases.Carts.Commands.Delete;
using DiamondShop.Application.Usecases.Carts.Commands.Update;
using DiamondShop.Application.Usecases.Carts.Queries.GetUserCart;
using DiamondShop.Domain.Models.AccountAggregate.Entities;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Carts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CartController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet]
        [Produces(typeof(List<CartItemDto>))]
        public async Task<ActionResult> Get([FromQuery]GetUserCartQuery getUserCartQuery)
        {
            var result = await _sender.Send(getUserCartQuery);
            var mappedResult = _mapper.Map<List<CartItemDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost]
        [Produces(typeof(List<CartItemDto>))]
        public async Task<ActionResult> Add(AddToCartCommand addToCartCommand)
        {
            var result = await _sender.Send(addToCartCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<CartItemDto>>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors,ModelState);
        }
        [HttpPut]
        [Produces(typeof(List<CartItemDto>))]
        public async Task<ActionResult> Update(UpdateCartItemCommand updateCartItemCommand)
        {
            var result = await _sender.Send(updateCartItemCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<CartItemDto>>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete]
        [Produces(typeof(List<CartItemDto>))]
        public async Task<ActionResult> Remove(RemoveFromCartCommand removeFromCartCommand)
        {
            var result = await _sender.Send(removeFromCartCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<CartItemDto>>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("{userId}/Checkout")]
        public async Task<ActionResult> Checkout(string userId)
        {
            return Ok();
        }
    }
}
