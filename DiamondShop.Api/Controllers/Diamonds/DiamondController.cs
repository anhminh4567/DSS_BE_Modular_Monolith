using DiamondShop.Application.Commons.Responses;
using DiamondShop.Application.Dtos.Responses.Diamonds;
using DiamondShop.Application.Usecases.Diamonds.Commands.Create;
using DiamondShop.Application.Usecases.Diamonds.Commands.Delete;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetAll;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetDetail;
using DiamondShop.Application.Usecases.Diamonds.Queries.GetPaging;
using DiamondShop.Domain.Models.Diamonds;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;
using System.Runtime.InteropServices;

namespace DiamondShop.Api.Controllers.Diamonds
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public DiamondController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(typeof(List<DiamondDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllDiamondQuery());
            var mappedResult = _mapper.Map<List<DiamondDto>>(result);
            return Ok(mappedResult);
        }
        [HttpGet("{id}")]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> Get([FromRoute] string id )
        {
            var command = new GetDiamondDetail(id);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
        [HttpGet]
        [Produces(typeof(PagingResponseDto<DiamondDto>))]
        public async Task<ActionResult> GetPaging([FromQuery]GetDiamondPagingQuery getDiamondPagingQuery)
        {
            var result = await _sender.Send(getDiamondPagingQuery);
            if(result.IsSuccess)
            {
                var mappedResult = _mapper.Map<PagingResponseDto<DiamondDto>>(result.Value);
                return Ok(mappedResult);
            }
                
            return MatchError(result.Errors, ModelState);
        }
        [HttpPost]
        [Produces(typeof(DiamondDto))]
        public async Task<ActionResult> Create([FromForm] CreateDiamondCommand createDiamondCommand)
        {
            var result = await _sender.Send(createDiamondCommand);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<DiamondDto>(result.Value);
                return Ok(mappedResult);
            }

            return MatchError(result.Errors, ModelState);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var command = new DeleteDiamondCommand(id);
            var result = await _sender.Send(command);
            if (result.IsSuccess)
                return Ok();
            return MatchError(result.Errors, ModelState);
        }
    }
}
