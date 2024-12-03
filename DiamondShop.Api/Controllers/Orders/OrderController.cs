using DiamondShop.Api.Controllers.Orders.AssignDeliverer;
using DiamondShop.Api.Controllers.Orders.Cancel;
using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Services.Interfaces;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Application.Usecases.Orders.Commands.DeliverFail;
using DiamondShop.Application.Usecases.Orders.Commands.Proceed;
using DiamondShop.Application.Usecases.Orders.Commands.Redeliver;
using DiamondShop.Application.Usecases.Orders.Commands.Refund;
using DiamondShop.Application.Usecases.Orders.Commands.Reject;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Customer;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Deliverer;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Manager;
using DiamondShop.Application.Usecases.Orders.Commands.Transfer.Staff;
using DiamondShop.Application.Usecases.Orders.Queries.GetAll;
using DiamondShop.Application.Usecases.Orders.Queries.GetOrderFilter;
using DiamondShop.Application.Usecases.Orders.Queries.GetPaymentLink;
using DiamondShop.Application.Usecases.Orders.Queries.GetUserOrderDetail;
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
        [HttpGet("All")]
        [Produces<PagingResponseDto<OrderDto>>]
        [Authorize]
        public async Task<ActionResult> GetAllOrder([FromQuery] OrderPaging orderPaging)
        {
            var userRole = User.FindFirst(IJwtTokenProvider.ROLE_CLAIM_NAME);
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userRole != null && userId != null)
            {
                var result = await _sender.Send(new GetAllOrderQuery(userRole.Value, userId.Value, orderPaging));
                var mappedResult = _mapper.Map<PagingResponseDto<OrderDto>>(result.Value);
                return Ok(mappedResult);
            }
            else
                return Unauthorized();
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<ActionResult> GetUserOrderDetail([FromRoute] string orderId)
        {
            var userRole = User.FindFirst(IJwtTokenProvider.ROLE_CLAIM_NAME);
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userRole != null && userId != null)
            {
                var result = await _sender.Send(new GetOrderDetailQuery(orderId, userRole.Value, userId.Value));
                var mappedResult = _mapper.Map<OrderDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return Unauthorized();
        }


        [HttpGet("PaymentLink/{orderId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetPaymentLink(string orderId)
        {
            var result = await _sender.Send(new GetOrderPaymentLink(orderId));
            if (result.IsFailed)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        #region Customer
        [HttpPost("Checkout")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> Checkout([FromBody] CheckoutRequestDto checkoutRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CheckoutOrderCommand(userId.Value, checkoutRequestDto.BillingDetail, checkoutRequestDto.CreateOrderInfo));
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPost("AddTransfer")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CustomerAddTransfer([FromForm] TransferVerifyRequestDto transferSubmitRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CustomerTransferCommand(userId.Value, transferSubmitRequestDto));
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }

        [HttpPut("ChangeEvidence")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CustomerChangeEvidence([FromQuery] ChangeEvidenceRequestDto changeEvidenceRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CustomerChangeEvidenceCommand(userId.Value, changeEvidenceRequestDto));
                if (result.IsSuccess)
                {
                    return Ok("Đã cập nhật bằng chứng");
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }


        [HttpPut("Cancel")]
        [Authorize(Roles = AccountRole.CustomerId)]
        public async Task<ActionResult> CustomerCancelOrder([FromQuery] string orderId, string reason)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new CancelOrderCommand(orderId, userId.Value, reason));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        #endregion
        #region Staff
        [HttpPut("Reject")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> ShopRejectOrder([FromQuery] string orderId, string reason)
        {
            var result = await _sender.Send(new RejectOrderCommand(orderId, reason));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }

        [HttpPut("AssignDeliverer")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> DeliveringOrder([FromQuery] AssignDelivererOrderCommand assignDelivererOrderCommand)
        {
            var result = await _sender.Send(assignDelivererOrderCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }

        [HttpPut("Redeliver")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> RedeliverOrder([FromQuery] RedeliverOrderCommand redeliverOrderCommand)
        {
            var result = await _sender.Send(redeliverOrderCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderDto>(result.Value);
                return Ok(mappedResult);
            }
            else
                return MatchError(result.Errors, ModelState);
        }

        #endregion
        [HttpPut("Proceed")]
        [Authorize(Roles = AccountRole.StaffId + "," + AccountRole.DelivererId)]
        public async Task<ActionResult> ProceedOrder([FromQuery] string orderId, [FromForm] DelivererCompleteOrderRequestDto? delivererCompleteOrder = null)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new ProceedOrderCommand(orderId, userId.Value, delivererCompleteOrder));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }

        [HttpPut("DeliverFail")]
        [Authorize(Roles = AccountRole.DelivererId)]
        public async Task<ActionResult> DeliverFail([FromQuery] string orderId)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new OrderDeliverFailCommand(orderId, userId.Value));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Deliverer/AddTransfer")]
        [Authorize(Roles = AccountRole.DelivererId)]
        public async Task<ActionResult> DeliverAddTransfer([FromQuery] TransferVerifyRequestDto transferConfirmRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new DelivererSendTransferCommand(userId.Value, transferConfirmRequestDto));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Deliverer/ChangeEvidence")]
        [Authorize(Roles = AccountRole.DelivererId)]
        public async Task<ActionResult> DeliverChangeEvidence([FromQuery] ChangeEvidenceRequestDto changeEvidenceRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new DelivererChangeEvidenceCommand(userId.Value, changeEvidenceRequestDto));
                if (result.IsSuccess)
                {
                    return Ok("Đã cập nhật bằng chứng");
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }

        [HttpPut("Staff/ConfirmTransfer/Pending")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> ConfirmPendingTransfer([FromBody] TransferConfirmRequestDto transferCompleteRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new StaffConfirmPendingTransferCommand(userId.Value, transferCompleteRequestDto));
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Staff/ConfirmTransfer/Delivering")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> ConfirmDeliveringTransfer([FromBody] TransferConfirmRequestDto transferCompleteRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new StaffConfirmDeliveringTransferCommand(userId.Value, transferCompleteRequestDto));
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPut("Staff/RejectTransfer")]
        [Authorize(Roles = AccountRole.StaffId)]
        public async Task<ActionResult> RejectTransfer([FromBody] TransferRejectRequestDto transferRejectRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new StaffRejectTransferCommand(userId.Value, transferRejectRequestDto));
                if (result.IsSuccess)
                {
                    return Ok(result.Value);
                }
                return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }

        [HttpPost("Manager/ConfirmTransfer/Refund")]
        [Authorize(Roles = AccountRole.ManagerId)]
        public async Task<ActionResult> CompleteOrderRefund([FromForm] RefundConfirmRequestDto refundConfirmRequestDto)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(new RefundOrderCommand(userId.Value, refundConfirmRequestDto));
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
                    return Ok(mappedResult);
                }
                else
                    return MatchError(result.Errors, ModelState);
            }
            else
                return Unauthorized();
        }
        [HttpPost("Transaction/Staff/Add")]
        [Authorize(Roles = $"{AccountRole.StaffId},{AccountRole.ManagerId}")]
        public async Task<ActionResult> AddTransactionForAtShopOrder([FromBody] StaffCreateTransactionForOrderCommand command)
        {
            var userId = User.FindFirst(IJwtTokenProvider.USER_ID_CLAIM_NAME);
            if (userId != null)
            {
                var result = await _sender.Send(command);
                if (result.IsSuccess)
                {
                    var mappedResult = _mapper.Map<OrderDto>(result.Value);
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
