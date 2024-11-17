using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.Discounts.Commands.Cancel;
using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.Discounts.Commands.CreateFull;
using DiamondShop.Application.Usecases.Discounts.Commands.Delete;
using DiamondShop.Application.Usecases.Discounts.Commands.Pause;
using DiamondShop.Application.Usecases.Discounts.Commands.SetThumbnail;
using DiamondShop.Application.Usecases.Discounts.Commands.Update;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateInfo;
using DiamondShop.Application.Usecases.Discounts.Commands.UpdateRequirements;
using DiamondShop.Application.Usecases.Discounts.Queries.GetAll;
using DiamondShop.Application.Usecases.Discounts.Queries.GetDetail;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Promotions
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiscountController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet]
        [Produces(type: typeof(List<DiscountDto>))]
        public async Task<ActionResult> GetAll()
        {
            var response = await _sender.Send(new GetAllDiscountQuery());
            var mappedResult = _mapper.Map<List<DiscountDto>>(response);
            return Ok(mappedResult);
        }

        [HttpGet("{discountId}")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> Get([FromRoute] string? discountId, [FromQuery] string? discountCode)
        {
            var response = await _sender.Send(new GetDiscountDetailQuery(discountId,discountCode));
            var mappedResult = _mapper.Map<DiscountDto>(response.Value);
            return Ok(mappedResult);
        }
        [HttpPost]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> Create(CreateDiscountCommand createDiscountCommand)
        {
            var result = await _sender.Send(createDiscountCommand);
            if(result.IsSuccess) 
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors,ModelState);
        }
        [HttpPost("Full")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> CreateFull(CreateFullDiscountCommand createFullDiscountCommand)
        {
            var result = await _sender.Send(createFullDiscountCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{discountId}/Requirement")]
        public async Task<ActionResult> UpdateRequiremwnt([FromRoute] string discountId, [FromBody] UpdateDiscountRequirementCommand updateDiscountRequirementCommand  )
        {
            var command = updateDiscountRequirementCommand with { discountId = discountId };
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{discountId}")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> UpdateInfo([FromRoute] string discountId, [FromBody] UpdateDiscountInfoCommand updateDiscountInfoCommand)
        {
            var command = updateDiscountInfoCommand with { discountId = discountId };
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("{discountId}/Full")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> UpdateFull([FromRoute] string discountId, [FromBody] UpdateDiscountCommand updateDiscountCommand)
        {
            var command = updateDiscountCommand with { discountId = discountId };
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }

        [HttpPut("{discountId}/Thumbnail")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> SetThumbnail([FromRoute] string discountId, [FromForm] SetDiscountThumbnailCommand setDiscountThumbnailCommand)
        {
            var command = setDiscountThumbnailCommand with { discountId = discountId };
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPatch("{discountId}/Pause")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> Pause([FromRoute]PauseDiscountCommand pauseDiscountCommand)
        {
            var result = await _sender.Send(pauseDiscountCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPatch("{discountId}/Cancel")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> Cancel([FromRoute] CancelDiscountCommand cancelDiscountCommand)
        {
            var result = await _sender.Send(cancelDiscountCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{discountId}")]
        [Produces(type: typeof(DiscountDto))]
        public async Task<ActionResult> Delete(string discountId )
        {
            var result = await _sender.Send(new DeleteDiscountCommand(discountId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiscountDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
