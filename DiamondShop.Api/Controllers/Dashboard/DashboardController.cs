using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Usecases.Dashboard.Queries;
using DiamondShop.Application.Usecases.Orders.Commands.Checkout;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Dashboard
{
    [Route("api/Dashboard")]
    [ApiController]
    [Authorize(Roles = AccountRole.StaffId)]
    public class DashboardController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DashboardController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        public async Task<ActionResult> GetDashboardInfo()
        {
            var result = await _sender.Send(new GetDashboardQuery());
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
