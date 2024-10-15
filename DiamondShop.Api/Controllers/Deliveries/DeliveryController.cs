using DiamondShop.Application.Usecases.Deliveries.Commands.Create;
using DiamondShop.Domain.Models.RoleAggregate;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Deliveries
{
    [Route("api/Delivery")]
    [ApiController]
    [Authorize]
    public class DeliveryController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DeliveryController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = AccountRole.AdminId)]
        public async Task<ActionResult> CreateDelivery([FromBody] CreateDeliveryCommand createDeliveryCommand)
        {
            var result = await _sender.Send(createDeliveryCommand);
            if (result.IsSuccess)
            {
                return Ok();
            }
            else
                return MatchError(result.Errors, ModelState);
        }
    }
}
