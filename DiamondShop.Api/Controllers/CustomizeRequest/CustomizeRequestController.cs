using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.CustomizeRequest;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.CustomRequest
{
    [Route("api/CustomizeRequest")]
    [ApiController]
    public class CustomizeRequestController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public CustomizeRequestController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("Admin/All")]
        public async Task<ActionResult> GetAll([FromQuery] GetAllCustomizeRequestQuery getAllCustomizeRequestQuery)
        {
            var result = await _sender.Send(getAllCustomizeRequestQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PagingResponseDto<CustomizeRequestDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Send")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> SendRequest([FromBody] CustomizeModelRequest customizeModelRequest)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new SendCustomizeRequestCommand(userId.Value, customizeModelRequest));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<CustomizeRequestDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
    }
}
