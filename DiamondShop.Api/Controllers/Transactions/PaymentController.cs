using DiamondShop.Application.Dtos.Responses.Transactions;
using DiamondShop.Application.Usecases.Transactions.Commands.UpdatePaymentMethod;
using DiamondShop.Application.Usecases.Transactions.Queries.GetAllMethods;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Transactions
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public PaymentController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(typeof(List<PaymentMethodDto>))]
        public async Task<ActionResult> GetAllPaymentMethod()
        {
            var result = await _sender.Send(new GetAllPaymentMethodQuery());
            return Ok(result);
        }
        [HttpPut("{methodId}")]
        [Produces(typeof(PaymentMethodDto))]
        public async Task<ActionResult> UpdatePaymentMethodStatus([FromRoute] string methodId, [FromBody] UpdatePaymentMethodRequestDto requestDto)
        {
            var command = new UpdatePaymentMethodCommand(methodId, requestDto.removeMaxPrice, requestDto.changeStatus, requestDto.setMaxPrice);
            var result = await _sender.Send(command);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PaymentMethodDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors,ModelState);
            throw new NotImplementedException();
        }
       
    }
}
