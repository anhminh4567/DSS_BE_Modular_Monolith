using DiamondShop.Application.Dtos.Responses.Orders;
using DiamondShop.Application.Usecases.OrderLogs.Command.CreateDeliveryLog;
using DiamondShop.Application.Usecases.OrderLogs.Command.CreateProcessingLog;
using DiamondShop.Application.Usecases.OrderLogs.Queries.GetAllOrderLogs;
using DiamondShop.Application.Usecases.OrderLogs.Queries.GetLogDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Orders
{
    [Route("api/Order/Log")]
    [ApiController]
    public class OrderLogController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public OrderLogController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderLog([FromRoute] string orderId)
        {
            var result = await _sender.Send(new GetAllOrderLogQuery(orderId));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors,ModelState);
        }
        [HttpGet("{orderId}/{logId}")]
        public async Task<ActionResult> GetOrderLogDetail([FromRoute] string orderId, [FromRoute] string logId)
        {
            var result = await _sender.Send(new GetLogDetailQuery(orderId,logId));
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("{orderId}/Processing")]
        public async Task<ActionResult> CreateProcessingLog([FromRoute] string orderId, [FromForm]string message, IFormFile[]? images)
        {
            var result = await _sender.Send(new CreateOrderProcessingLogCommand(orderId, message, images));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderLogDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("{orderId}/Delivering")]
        public async Task<ActionResult> CreateDeliveringLog([FromRoute] string orderId, [FromForm] string message, IFormFile[]? images)
        {
            var result = await _sender.Send(new CreateOrderDeliveryLogCommand(orderId, message, images));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<OrderLogDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
