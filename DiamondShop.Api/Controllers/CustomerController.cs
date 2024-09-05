using DiamondShop.Application.Usecases.Customers.Commands.Security.DownRankCustomer;
using DiamondShop.Application.Usecases.Customers.Commands.Security.UpRankCustomer;
using DiamondShop.Application.Usecases.Customers.Queries.GetCustomerDetail;
using DiamondShop.Application.Usecases.Customers.Queries.GetCustomerPage;
using DiamondShop.Domain.Models.CustomerAggregate.ValueObjects;
using DiamondShop.Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ApiControllerBase
    {
        private readonly ISender _sender;

        public CustomerController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet]
        public async Task<ActionResult> GetPaging([FromQuery]GetCustomerPageQuery getCustomerPageQuery)
        {
            return Ok((await _sender.Send(getCustomerPageQuery)).Value);
        }

    
        [HttpGet("Detail")]
        [Authorize]
        public async Task<ActionResult> GetDetail()
        {
            var result = await _sender.Send(new GetCustomerDetailQuery());
            if(result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors,ModelState);
        }
        [HttpPut("Uprank")]
        //[Consumes("applicaton/json")]
        //[Authorize(Roles = )]
        public async Task<ActionResult> UpRank([FromBody] UpRankCustomerCommand upRankCustomerCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(upRankCustomerCommand, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("DownRank")]
        //[Consumes("applicaton/json")]
        //[Authorize(Roles = )]
        public async Task<ActionResult> DownRank([FromBody] DownRankCustomerCommand downRankCustomerCommand, CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(downRankCustomerCommand, cancellationToken);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }

    }
}
