using DiamondShop.Application.Dtos.Responses.JewelryModels;
using DiamondShop.Application.Usecases.JewelryModels.Commands.Create;
using DiamondShop.Application.Usecases.JewelryModels.Queries.GetAll;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DiamondShop.Api.Controllers.JewelryModels
{
    [Route("api/JewelryModel")]
    [ApiController]
    public class JewelryModelController : ApiControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        public JewelryModelController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }
        [HttpGet("All")]
        [Produces(type: typeof(List<JewelryModelDto>))]
        public async Task<ActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllJewelryModelQuery());
            var mappedResult = _mapper.Map<List<JewelryModelDto>>(result);
            return Ok(mappedResult);
        }
        [HttpPost("Create")]
        [Produces(type: typeof(JewelryModelDto))]
        public async Task<ActionResult> Create([FromBody] CreateJewelryModelCommand command)
        {
            var result = await _sender.Send(command);
            if (result.IsSuccess)
            {
                var mappedResult = _mapper.Map<JewelryModelDto>(result.Value);
                return Ok(mappedResult);
            }
            return MatchError(result.Errors, ModelState);
        }
    }
}
