using DiamondShop.Application.Usecases.Customers.Queries.GetCustomerPage;
using MediatR;
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
        public async Task<ActionResult> GetPaging(GetCustomerPageQuery getCustomerPageQuery)
        {
            return Ok((await _sender.Send(getCustomerPageQuery)).Value);
        } 

    }
}
