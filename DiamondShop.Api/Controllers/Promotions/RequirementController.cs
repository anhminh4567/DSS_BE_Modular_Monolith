using DiamondShop.Application.Dtos.Responses.Promotions;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.CreateMany;
using DiamondShop.Application.Usecases.PromotionRequirements.Commands.Delete;
using DiamondShop.Application.Usecases.PromotionRequirements.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Promotions
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public RequirementController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet()]
        [Produces(type: typeof(List<RequirementDto>))]
        public async Task<ActionResult> GetAllRequirements()
        {
            var result = await _sender.Send(new GetAllRequirementQuery());
            var mappedResult = _mapper.Map<List<RequirementDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost()]
        [Produces(type: typeof(List<RequirementDto>))]
        public async Task<ActionResult> CreateRequirement(CreateRequirementCommand createRequirementCommand)
        {
            var result = await _sender.Send(createRequirementCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<RequirementDto>>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{requirementId}")]
        [Produces(type: typeof(RequirementDto))]
        public async Task<ActionResult> DeleteRequirement(string requirementId)
        {
            var result = await _sender.Send(new DeleteRequirementCommand(requirementId));
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<RequirementDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
