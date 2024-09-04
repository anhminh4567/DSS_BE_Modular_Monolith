using DiamondShop.Application.Usecases.Customers.Commands.RegisterCustomer;
using DiamondShop.Application.Usecases.Customers.Queries.LoginCustomer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ApiControllerBase
    {
        private readonly ISender _sender;

        public AuthenticationController(ISender sender)
        {
            _sender = sender;
        }
        [HttpPost("register")]
        [Consumes("application/json")]
        public async Task<ActionResult> Register(RegisterCustomerCommand registerCustomerCommand)
        {
            var result = await _sender.Send(registerCustomerCommand);
            if(result.IsSuccess is false) 
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }
        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] LoginCustomerQuery loginCustomerQuery)
        {
            var result = await _sender.Send(loginCustomerQuery);
            if(result.IsSuccess is false)
                return MatchError(result.Errors, ModelState);
            return Ok(result.Value);
        }

    }
}
