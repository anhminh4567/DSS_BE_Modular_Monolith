using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.Discounts.Commands.Create;
using DiamondShop.Application.Usecases.Discounts.Commands.Delete;
using DiamondShop.Application.Usecases.Discounts.Commands.Update;
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
        public async Task<ActionResult> GetAll()
        {
            var response = await _sender.Send(new GetAllDiscountQuery());
            var mappedResult = _mapper.Map<List<DiscountDto>>(response);
            return Ok(mappedResult);
        }

        [HttpGet("{discountId}")]
        public async Task<ActionResult> Get([FromRoute] string discountId)
        {
            var response = await _sender.Send(new GetDiscountDetailQuery(discountId));
            var mappedResult = _mapper.Map<DiscountDto>(response.Value);
            return Ok(mappedResult);
        }
        [HttpPost]
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
        [HttpPut("{discountId}")]
        public async Task<ActionResult> Update([FromRoute]string discountId, [FromBody] UpdateDiscountCommand updateDiscountCommand )
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
        [HttpDelete("{discountId}")]
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
