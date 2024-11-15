using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.CustomizeRequest;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Checkout;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Customer;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Proceed.Staff;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Customer;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.Reject.Staff;
using DiamondShop.Application.Usecases.CustomizeRequests.Commands.SendRequest;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetAll;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomer;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetCustomerDetail;
using DiamondShop.Application.Usecases.CustomizeRequests.Queries.GetDetail;
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
        #region Staff,Admin
        [HttpGet("Staff/All")]
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
        [HttpGet("Staff/Detail")]
        public async Task<ActionResult> GetDetail([FromQuery] GetDetailCustomizeRequestQuery getDetailCustomizeRequestQuery)
        {
            var result = await _sender.Send(getDetailCustomizeRequestQuery);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<CustomizeRequestDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Staff/Proceed")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> StaffProceedCustomizeRequest([FromBody] StaffProceedCustomizeRequestCommand proceedCustomizeRequestCommand)
        {
            var result = await _sender.Send(proceedCustomizeRequestCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<CustomizeRequestDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Staff/Reject")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> StaffRejectCustomizeRequest([FromQuery] StaffRejectRequestCommand staffRejectRequestCommand)
        {
            var result = await _sender.Send(staffRejectRequestCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<CustomizeRequestDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }
        #endregion
        #region Customer
        [HttpGet("Customer/All")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> GetCustomerRequest([FromQuery] GetCustomerRequestDto getCustomerRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new GetCustomerCustomizeRequestQuery(userId.Value, getCustomerRequestDto));
                var mappedResult = _mapper.Map<PagingResponseDto<CustomizeRequestDto>>(result);
                return Ok(mappedResult);
            }
            else
                return Unauthorized();
        }
        [HttpGet("Customer/Detail")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> GetCustomerRequest([FromQuery] string requestId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new GetCustomerDetailCustomizeRequestQuery(requestId, userId.Value));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<CustomizeRequestDto>(result.Value);
                    return Ok(mappedResult);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPost("Send")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> SendRequest([FromBody] CustomizeModelRequest customizeModelRequest)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CreateCustomizeRequestCommand(userId.Value, customizeModelRequest));
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
        [HttpPost("Checkout")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CheckoutCustomizeRequest([FromBody] CheckoutCustomizeRequestDto checkoutCustomizeRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CheckoutRequestCommand(userId.Value, checkoutCustomizeRequestDto));
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
        [HttpPut("Proceed")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CustomerProceedCustomizeRequest([FromQuery] string CustomizeRequestId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CustomerRequestingRequestCommand(CustomizeRequestId, userId.Value));
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
        [HttpPut("Reject")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CheckRequestPrice([FromQuery] string CustomizeRequestId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CustomerRejectRequestCommand(CustomizeRequestId, userId.Value));
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
        #endregion
    }
}
