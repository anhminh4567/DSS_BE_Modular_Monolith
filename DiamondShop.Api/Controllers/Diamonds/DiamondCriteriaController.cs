using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.Create;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.CreateFromRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.Delete;
using DiamondShop.Application.Usecases.DiamondCriterias.Commands.UpdateRange;
using DiamondShop.Application.Usecases.DiamondCriterias.Queries.GetAll;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Domain.Models.DiamondPrices.Entities;
using DiamondShop.Domain.Models.Diamonds;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.Diamonds
{
    [Route("api/Diamond/Criteria")]
    [ApiController]
    public class DiamondCriteriaController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondCriteriaController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(typeof(List<DiamondCriteriaDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllDiamondCriteriaQuery());
            var mappedResult = _mapper.Map<List<DiamondCriteriaDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost]
        [Produces(typeof(DiamondCriteriaDto))]
        public async Task<ActionResult> Create([FromBody] CreateDiamondCriteriaCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondCriteriaDto>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Range/MainDiamond")]
        [Produces(typeof(DiamondCriteriaDto))]
        public async Task<ActionResult> CreateFromRange([FromBody] CreateCriteriaFromRangeCommand command)
        {
            var result = await _sender.Send(command with { IsSideDiamond = false });
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondCriteriaDto>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost("Range/SideDiamond")]
        [Produces(typeof(DiamondCriteriaDto))]
        public async Task<ActionResult> CreateFromRangeSideDimamond([FromBody] CreateCriteriaFromRangeCommand command)
        {
            var result = await _sender.Send(command with { IsSideDiamond = true});
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondCriteriaDto>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpPut("Range")]
        [Produces(typeof(DiamondCriteriaDto))]
        public async Task<ActionResult> UpdateFromRange([FromBody] UpdateDiamondCriteriaRangeCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<List<DiamondCriteriaDto>>(result);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{criteriaId}")]
        public async Task<ActionResult> Delete( string criteriaId)
        {
            var result = await _sender.Send(new DeleteDiamondCriteriaCommand(criteriaId));
            if (result.IsSuccess)
            {
                return Ok();
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
