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
        public async Task<ActionResult> GetAllPaymentMethod()
        {
            var result = await _sender.Send(new GetAllPaymentMethodQuery());
            return Ok(result);
        }
        [HttpPut("{methodId}")]
        public async Task<ActionResult> UpdatePaymentMethodStatus([FromRoute] string methodId)
        {
            throw new NotImplementedException();
        }
       
    }
}
