using DiamondShop.Application.Dtos.Requests.Orders;
using DiamondShop.Application.Dtos.Responses.Accounts;
using DiamondShop.Application.Usecases.Accounts.Queries.GetCounts;
using DiamondShop.Application.Usecases.Dashboard.Queries;
using DiamondShop.Application.Usecases.Dashboard.Queries.GetOrderCompleted;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingCaratRangeForShape;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingForManyShape;
using DiamondShop.Application.Usecases.Diamonds.Queries.DashBoard.GetBestSellingJewelry;
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
        [HttpGet("OrderCompleted/Count")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCompletedOrderByDateRange([FromQuery] string? startDate, [FromQuery] string? endDate , [FromQuery] bool? isCustomOrder )
        {
            var query = new GetOrderCompletedByRangeQuery(startDate, endDate, isCustomOrder);
            var result = await _sender.Send(query);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Account/Count")]
        [Produces(typeof(int))]
        public async Task<ActionResult> GetAllDelivereAndStatus([FromQuery] List<string> roles)
        {
            var result = await _sender.Send(new GetAccountCountInRolesQuery(roles));
            return Ok(result);
        }
        [HttpGet("Diamond/TopSelling/AllShape")]
        public async Task<ActionResult> GetTopSellingFromAllShape([FromQuery] string? startDate, [FromQuery] string? endDate, [FromQuery] bool? isLab)
        {
            var command = new GetBestSellingForShapeQuery(startDate, endDate, isLab);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Diamond/TopSelling/{shapeId}")]
        public async Task<ActionResult> GetTopSellingFromAllShape([FromRoute] string shapeId,
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            [FromQuery] float caratFrom,
            [FromQuery] float caratTo,
            [FromQuery] bool? isLab)
        {
            var command = new GetBestSellingCaratRangeForShapeQuery(shapeId, caratFrom, isLab, caratTo , startDate, endDate);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet("Jewelry/TopSelling")]
        public async Task<ActionResult> GetTopSellingJewelry()
        {
            var result = await _sender.Send(new GetBestSellingJewelryQuery());
            if (result.IsSuccess)
                return Ok(result.Value);
            return MatchError(result.Errors, ModelState);
        }
    }
}
