using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.Metals.Commands.Update;
using DiamondShop.Application.Usecases.Metals.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/JewelryModel/Metal")]
    [ApiController]
    public class MetalController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public MetalController(ISender sender, IMapper mapper) 
        { 
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(type:typeof(List<MetalDto>))]
        public async Task<ActionResult> GetAll()
        {
            var command = new GetAllMetalQuery();
            var result = await _sender.Send(command);
            var mappedResult = _mapper.Map<List<MetalDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPut("/UpdatePrice")]
        public async Task<ActionResult> UpdatePrice([FromBody] UpdateMetalCommand updateMetalCommand)
        {
            var result = await _sender.Send(updateMetalCommand);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
