using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Deliveries.Commands.Begin;
using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Application.Usecases.Deliveries.Queries.GetAll;
using DiamondShop.Application.Usecases.Deliveries.Queries.GetDetail;
using DiamondShop.Application.Usecases.Deliveries.Queries.GetUser;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Deliveries
{
    [Route("api/Delivery")]
    [ApiController]
    [Authorize]
    public class DeliveryController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DeliveryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> GetAllDelivery()
        {
            var result = await _sender.Send(new GetAllDeliveryCommand());
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<DeliveryPackageDto>>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }

        [HttpGet("User/All")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> GetUserDelivery()
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new GetUserDeliveryCommand(userId.Value));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<List<DeliveryPackageDto>>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpGet("User/{deliveryId}")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> GetUserDelivery([FromRoute] string deliveryId)
        {
            var result = await _sender.Send(new GetDeliveryDetailCommand(deliveryId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DeliveryPackageDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Create")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> CreateDelivery([FromBody] CreateDeliveryCommand createDeliveryCommand)
        {
            var result = await _sender.Send(createDeliveryCommand);
            if (result.IsSuccess)
            {

                return Ok("Delivery created and is assigned to staff");
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Begin")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> BeginDelivery()
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new BeginDeliveryCommand(userId.Value));
                if (result.IsSuccess)
                {

                    return Ok("Delivery begins");
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
    }
}
